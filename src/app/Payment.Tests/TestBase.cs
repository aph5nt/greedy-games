using System;
using System.IO;
using Akka.Actor;
using Akka.Configuration;
using AutoMapper;
using NUnit.Framework;
using Payment.Contracts.Providers;
using Serilog;
using WebApi.Configuration;
using WebApi.Providers.Impl;

namespace AppServer.Tests
{
    public class TestBase
    {
        protected SetUpTests SetUp;
        protected ActorSystem System;
        protected IActorRef DepositActorRef;
        protected IActorRef DepositConfirmationActorRef;
        protected IActorRef DepositSchedulerActorRef;
        protected IActorRef BalanceActorRef;
        protected IActorRef TransactionManagerRef;
        protected TransactionActorHelper TransactionActorHelper;
        protected IMapper Mapper;
        protected const string ServerAddress = "akka.tcp://AppServer@localhost:8085";
 
        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
            
            SetUp = SetUpTests.HostInstance();
 
            var cfg = File.ReadAllText(Path.Combine(AppService.ExecutableDirectory, "TestSystem.hocon"));
            System = ActorSystem.Create("TestSystem", ConfigurationFactory.ParseString(cfg));

            DepositActorRef = System.ActorSelection($"{ServerAddress}/user/deposit-scheduler/deposit")
                .ResolveOne(TimeSpan.FromSeconds(30)).Result;

            DepositConfirmationActorRef = System.ActorSelection($"{ServerAddress}/user/deposit-scheduler/confirm")
                .ResolveOne(TimeSpan.FromSeconds(30)).Result;

            DepositSchedulerActorRef = System.ActorSelection($"{ServerAddress}/user/deposit-scheduler")
                .ResolveOne(TimeSpan.FromSeconds(30)).Result;

            BalanceActorRef = System.ActorSelection($"{ServerAddress}/user/transaction/free/balance")
                .ResolveOne(TimeSpan.FromSeconds(30)).Result;

            TransactionManagerRef = System.ActorSelection($"{ServerAddress}/user/transaction")
                .ResolveOne(TimeSpan.FromSeconds(30)).Result;

            TransactionActorHelper = new TransactionActorHelper(new RemoteTransactionManagerProvider(System,
                new WebSettings
                {
                    AkkaSettings = new AkkaSettings
                    {
                        ActorTimeout = 30,
                        AppServerUrl = ServerAddress
                    }
                }));
        }

        [SetUp]
        public virtual void Setup()
        {

            // CleanUpDatabase();
        }
    }
}