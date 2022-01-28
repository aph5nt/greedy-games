using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Persistance;

namespace AppServer.Tests.Transactions
{
    [SetUpFixture]
    public class TransactionsSetup
    {
        protected SetUpTests SetUp;

        [OneTimeSetUp]
        public void Setup()
        {
            SetUp = SetUpTests.HostInstance();

            var options = new DbContextOptionsBuilder<DataContext>().UseSqlServer(SetUp.Configuration["ConnectionString:Sql"]).Options;

            using (var context = new DataContext(options))
            {
                context.Database.Migrate();
                context.TransactionLogs.RemoveRange(context.TransactionLogs);
                context.SaveChanges();
            }
        }
    }
}