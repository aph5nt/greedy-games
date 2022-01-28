using System;
using System.IO;
using System.Reflection;
using AppServer.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using Payment.Services;
using Persistance;
using Persistance.Model.Payments;
using Shared.Configuration;
using Shared.Model;

namespace AppServer.Tests
{
    [SetUpFixture]
    // ReSharper disable once CheckNamespace
    public class SetUpTests
    {
        private static readonly object LockSync = new object();
        private static SetUpTests _instance;
        public IConfigurationRoot Configuration { get; set; }
        public AppServerSettings AppServerSettings = new AppServerSettings();
        public Mock<IWavesApiFactory> WavesApiFactoryMock { get; set; } = new Mock<IWavesApiFactory>();
        private AppService AppService { get; set; }

        public static SetUpTests HostInstance()
        {
            if (_instance == null)
            {
                lock (LockSync)
                {
                    if (_instance == null)
                    {
                        _instance = new SetUpTests();
                    }
                }
            }

            return _instance;
        }

        public DataContext GetDataContext()
        {
            return new DataContext(new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer(Configuration["ConnectionString:Sql"]).Options);
        }
 

        [OneTimeSetUp]
        public void SetUp()
        {
            _instance = this;
             
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appserver.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appserver.Test.json", optional: false, reloadOnChange: false);

            Configuration = builder.Build();
            Configuration.Bind(AppServerSettings);

            GetDataContext()
                .Database
                .Migrate();

            CleanupDatabase();

            SeedData(GetDataContext());

            AppService = new AppService();
            AppService.Start(AppServerSettings,
                Configuration,
                new TransactionModule(AppServerSettings.ConnectionString.Sql, WavesApiFactoryMock.Object),
                new TransactionJobModue(AppServerSettings.ConnectionString.Sql),
                new MinefielGameModule());
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            AppService.Stop();
            AppService = null;

            // AzureStorageProcess.TryStop();

            Configuration = null;
        }

        private void CleanupDatabase()
        {
            using (var context = GetDataContext())
            {
                context.TransactionLogs.RemoveRange(context.TransactionLogs);
                context.SaveChanges();
            }
        }

        private void SeedData(DataContext context)
        {
            context.TransactionLogs.AddRange(
                new TransactionLog(Network.FREE, $"{Balances.BalanceActorTests.UserName}1", LogEventType.CreateAccount,
                    100 * Money.Sathoshi, Guid.NewGuid())
                {
                    Balance = 100 * Money.Sathoshi
                },
                new TransactionLog(Network.FREE, $"{Balances.BalanceActorTests.UserName}2", LogEventType.CreateAccount,
                    100 * Money.Sathoshi, Guid.NewGuid())
                {
                    Balance = 100 * Money.Sathoshi
                },
                new TransactionLog(Network.FREE, $"{Balances.BalanceActorTests.UserName}3", LogEventType.CreateAccount,
                    100 * Money.Sathoshi, Guid.NewGuid())
                {
                    Balance = 100 * Money.Sathoshi
                }
            );

            context.SaveChanges();
        }
    }
}