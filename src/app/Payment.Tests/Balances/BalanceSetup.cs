using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Persistance;

namespace AppServer.Tests.Balances
{
    [SetUpFixture]
    public class BalancesSetup
    {
        protected SetUpTests SetUp;

        [OneTimeSetUp]
        public void Setup()
        {
            SetUp = SetUpTests.HostInstance();

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer(SetUp.Configuration["ConnectionString:Sql"]).Options;

            using (var context = new DataContext(options))
            {
                context.Database.Migrate();
                    
                
                {
                    //context.UserLogins.RemoveRange(context.UserLogins);
                    //context.UserClaims.RemoveRange(context.UserClaims);
                    //context.UserTokens.RemoveRange(context.UserTokens);
                    //context.UserRoles.RemoveRange(context.UserRoles);
                    //context.Users.RemoveRange(context.Users);
                    //context.SaveChanges();
                }

                // var account = AccountRepository.Get(message.Identity.Network, message.Identity.UserName);
                // create account

                // mock IWavesApiFactory


            }

            // setup some accounts here eg. bank roll acconut, user test account itp itp.

        }
    }
}