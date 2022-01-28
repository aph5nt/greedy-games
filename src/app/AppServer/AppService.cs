using System;
using System.Collections.Specialized;
using System.Diagnostics;
using Akka.Actor;
using Akka.Configuration;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Akka.Routing;
using Akka.Util.Internal;
using AppServer.Modules;
using AppServer.Providers;
using Autofac;
using Autofac.Core;
using Game.Minefield.Actors;
using Game.Minefield.Contracts.Services;
using Game.Minefield.Storage.Impl;
using Microsoft.Extensions.Configuration;
using Payment.Actors;
using Payment.Actors.Jobs;
using Quartz;
using Quartz.Impl;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.RollingFileAlternate;
using Shared.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Akka.Dispatch.SysMsg;
using AutoMapper;
using Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Payment.Contracts.DataTransfer;
using Persistance;
using Persistance.Model.Accounts;
using Persistance.Model.Payments;
using Persistance.Repositories;
using Shared.Model;

namespace AppServer
{
    public class AppService
    {
        private static ActorSystem _system;
        public static Quartz.IScheduler Scheduler;
        private static IContainer _container;
        private IDependencyResolver _dependencyResolver;
        public static readonly string ExecutableDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

        public async void Start(AppServerSettings settings, IConfiguration configuration, params IModule[] modules)
        {
            var loggerConfiguration =
                new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.ColoredConsole()
                    .WriteTo.RollingFileAlternate(Path.Combine(ExecutableDirectory, "logs"), "appserver", LogEventLevel.Debug);

            //loggerConfiguration.WriteTo.Seq("http://localhost:5341", LogEventLevel.Debug);
            if (configuration["LoggerUrl"] != null)
            {
                loggerConfiguration.WriteTo.Seq(configuration["LoggerUrl"], LogEventLevel.Debug);
            }

            Log.Logger = loggerConfiguration.CreateLogger();
            
            Serilog.Debugging.SelfLog.Enable(message => {
                Console.WriteLine(message);
            });
            
            Serilog.Debugging.SelfLog.Enable(Console.Error);
                
            Log.Information("LoggerUrl: " + configuration["LoggerUrl"]);
            Log.Information("Akka.PublicHostname: " + settings.AkkaSettings.PublicHostname);
            Log.Information("Akka.Hostname: " + settings.AkkaSettings.Hostname);
            Log.Information("Akka.Port: " + settings.AkkaSettings.Port);
 
            var cfg = File.ReadAllText(Path.Combine(ExecutableDirectory, "AppServer.hocon"));
            cfg = cfg.Replace("#port", settings.AkkaSettings.Port.ToString());
            cfg = cfg.Replace("#hostname", settings.AkkaSettings.Hostname);
            cfg = cfg.Replace("#publicHostname", settings.AkkaSettings.PublicHostname);
            _system = ActorSystem.Create("AppServer", ConfigurationFactory.ParseString(cfg));

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance(_system).As<IActorRefFactory>().SingleInstance();
            containerBuilder.RegisterInstance(settings).AsSelf().SingleInstance();
            containerBuilder.RegisterInstance<IConfiguration>(configuration).SingleInstance();
            containerBuilder.RegisterInstance<IBalanceProvider>(new BalanceProvider(_system)).SingleInstance();

            modules.ForEach(m => containerBuilder.RegisterModule(m));

            _container = containerBuilder.Build();
            _dependencyResolver = new AutoFacDependencyResolver(_container, _system);

            RunMigrations(_container);
            
            _system.ActorOf(_dependencyResolver.Create<TransactionManagerActor>(), "transaction");
            _system.ActorOf(_dependencyResolver.Create<WavesGatewayActor>(), "payment-gateway");
            _system.ActorOf(_dependencyResolver.Create<UserWithdrawActor>().WithRouter(new RoundRobinPool(1, new DefaultResizer(1, 10))), "user-withdraw");
            _system.ActorOf(_dependencyResolver.Create<GameManagerActor>(), "minefield");
            _system.ActorOf(Props.Create<ChatHubActor>(), "chathub");
            
            new GameStorage(settings).InitAsync().Wait();
            new LogStorage(settings).InitAsync().Wait();

          
            InitBankAccounts(settings);
 
            Scheduler = await new StdSchedulerFactory(new NameValueCollection
                {
                    {"quartz.serializer.type", "binary"},
                    {"quartz.dataSource.default.connectionString", settings.ConnectionString.Sql},
                    {"quartz.dataSource.default.provider", "SqlServer"},
                    {"quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz"},
                    {"quartz.jobStore.useProperties", "false"},
                    {"quartz.jobStore.tablePrefix", "QRTZ_"},
                    {"quartz.jobStore.dataSource", "default"},
                })
                .GetScheduler();

            await Scheduler.Start();

            DepositSchedulerActor.Init(_system, Scheduler);
            DividendSchedulerActor.Init(_system, Scheduler);
            UserWithdrawSchedulerActor.Init(_system, Scheduler);

            _system.ActorOf(Props.Create<EventListenerActor>(), "events");
            NotificationManagerActor.Init(_system, settings);
        }

        private void RunMigrations(IContainer container)
        {
            using (var context = container.Resolve<DataContext>())
            {
                context.Database.Migrate();    
            }
        }

        public void Start()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appserver.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfiguration configuration = builder.Build();
            
            var keyVault = configuration.GetSection("KeyVault");
            builder.AddAzureKeyVault(
                $"https://{keyVault["Vault"]}.vault.azure.net/",
                keyVault["ClientId"],
                keyVault["ClientSecret"], new DefaultKeyVaultSecretManager());

            configuration = builder.Build();
            
            var settings = new AppServerSettings();
            configuration.Bind(settings);
 
            Start(settings,
                configuration,
                new TransactionModule(settings.ConnectionString.Sql),
                new TransactionJobModue(settings.ConnectionString.Sql),
                new MinefielGameModule());
        }

        private static void InitBankAccounts(AppServerSettings settings)
        {
            foreach (var paymentsNetwork in settings.Payments.Networks)
            {
                if (String.IsNullOrWhiteSpace(paymentsNetwork.BankAddress) ||
                    String.IsNullOrWhiteSpace(paymentsNetwork.BankDepositAddress) ||
                    String.IsNullOrWhiteSpace(paymentsNetwork.DividendAddress) ||
                    String.IsNullOrWhiteSpace(paymentsNetwork.ProfitAddress))
                {
                    throw new NotImplementedException();
                }
                
                var db = _container.Resolve<DataContext>();
                
                var account = db.Accounts.OfType<GameAccount>().SingleOrDefault(x =>
                    x.Network == paymentsNetwork.Network && x.UserName == GameTypes.Minefield.ToString());

                if (account == null)
                {
                    db.Accounts.Add(new GameAccount
                    {
                        DepositAddress = paymentsNetwork.BankAddress,
                        Network = paymentsNetwork.Network,
                        UpdatedAt = DateTime.UtcNow,
                        Treshold = Money.Sathoshi * 1000,
                        UserName = GameTypes.Minefield.ToString()
                    });
                    
                    db.Accounts.Add(new GameAccount
                    {
                        DepositAddress = paymentsNetwork.BankDepositAddress,
                        Network = paymentsNetwork.Network,
                        UpdatedAt = DateTime.UtcNow,
                        UserName = GameTypes.Minefield + "Deposit"
                    });

                    db.SaveChanges();
                }
            }
        }

        public void Start(IConfiguration configuration)
        {
            var settings = new AppServerSettings();
            configuration.Bind(settings);

            Start(settings,
                configuration,
                new TransactionModule(settings.ConnectionString.Sql),
                new TransactionJobModue(settings.ConnectionString.Sql),
                new MinefielGameModule(),
                new ChatModule());
        }

        public void Stop()
        {
            CoordinatedShutdown.Get(_system).Run().Wait();
            Scheduler.Shutdown(false);
        }
    }
}