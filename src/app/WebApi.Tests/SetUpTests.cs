using AppServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Persistance;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using WebApi.Data;

namespace WebApi.Tests
{
    [SetUpFixture]
    // ReSharper disable once CheckNamespace
    public class SetUpTests
    {
        private static readonly object LockSync = new object();
        private static SetUpTests _instance;
        public HttpClient Client { get; set; }
        public IConfiguration Configuration { get; set; }
        private TestServer Server { get; set; }
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
                .UseSqlServer(Configuration["ConnectionString"]).Options);
        }

        public ApplicationDbContext GetApplicationDbContext()
        {
            return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(Configuration["ConnectionString"]).Options);
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            _instance = this;

            var builder = new ConfigurationBuilder().AddCommandLine(new[] {"--environment=Test"})
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("webapi.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appserver.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appserver.Test.json", optional: false, reloadOnChange: false)
                .AddJsonFile("webapi.Test.json", optional: false, reloadOnChange: false);

            Configuration = builder.Build();

            var keyVault = Configuration.GetSection("KeyVault");
            builder.AddAzureKeyVault(
                $"https://{keyVault["Vault"]}.vault.azure.net/",
                keyVault["ClientId"],
                keyVault["ClientSecret"], new DefaultKeyVaultSecretManager());

            Configuration = builder.Build();
            
            GetDataContext()
                .Database
                .Migrate();

            GetApplicationDbContext()
                .Database
                .Migrate();
 
            AppService = new AppService();
            AppService.Start(Configuration);

            Server = new TestServer(new WebHostBuilder().UseConfiguration(Configuration).UseStartup<Startup>());

            Client = Server.CreateClient();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Client.Dispose();
            Client = null;

            Server.Host.StopAsync(TimeSpan.FromSeconds(60)).Wait();
            Server.Dispose();
            Server = null;

            AppService.Stop();
            AppService = null;

            Configuration = null;
        }
    }
}