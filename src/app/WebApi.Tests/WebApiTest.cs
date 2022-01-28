using System.Net.Http;
using NUnit.Framework;
using Persistance;

namespace WebApi.Tests
{
    public class WebApiTest
    {
        protected HttpClient Client;

        protected SetUpTests SetUp;

        public void CleanUpDatabase()
        {
            //using (var context = new Entities())
            //{
            //    // context.Database.ExecuteSqlCommand("delete from dbo.Customers");
            //    context.Database.ExecuteSqlCommand("delete from dbo.Rules");
            //    context.Database.ExecuteSqlCommand("delete from dbo.Items");
            //    context.Database.ExecuteSqlCommand("delete from dbo.Things");
            //}
        }
         
        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            SetUp = SetUpTests.HostInstance();
            Client = SetUp.Client;
        }

        [SetUp]
        public virtual void Setup()
        {
            CleanUpDatabase();
        }
    }
}