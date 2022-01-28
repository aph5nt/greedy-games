using System;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Persistance;
using Persistance.Model.Accounts;
using Shared.Model;

namespace AppServer.Tests.Deposits
{
    [SetUpFixture]
    public class DepositSetup
    {
        protected SetUpTests SetUp;
        public static UserAccount UserAccount;
        public static GameAccount GameAccount;

        [OneTimeSetUp]
        public void Setup()
        {
            SetUp = SetUpTests.HostInstance();

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer(SetUp.Configuration["ConnectionString:Sql"]).Options;

            using (var context = new DataContext(options))
            {
                context.Database.Migrate();
                context.Deposits.RemoveRange(context.Deposits);
                context.Accounts.RemoveRange(context.Accounts);
                context.SaveChanges();


                UserAccount = new UserAccount
                {
                    Network = Network.WAVES,
                    UserName = "aph5n5",
                    DepositAddress = "addr1",
                    IsActive = true,
                    UpdatedAt = DateTime.UtcNow
                };

                GameAccount = new GameAccount
                {
                    Network = Network.WAVES,
                    UserName = GameTypes.Minefield.ToString(),
                    DepositAddress = "gamedepositaddr",
                    Treshold = 1000 * Money.Sathoshi,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Accounts.Add(UserAccount);
                context.Accounts.Add(GameAccount);
                context.SaveChanges();
            }
        }
    }
}