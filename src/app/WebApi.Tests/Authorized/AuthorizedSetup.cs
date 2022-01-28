using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net.Http;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Tests.Authorized
{
    [SetUpFixture]
    public class AuthorizedSetup
    {
        public static string UserName;
        public static string Password;
        public static string Token;

        protected HttpClient Client;

        protected SetUpTests SetUp;

        [OneTimeSetUp]
        public void Setup()
        {
            SetUp = SetUpTests.HostInstance();
            Client = SetUp.Client;

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(SetUp.Configuration["ConnectionString"]).Options;

            using (var context = new ApplicationDbContext(options))
            {
                if (!context.Database.EnsureCreated())
                {
                    context.UserLogins.RemoveRange(context.UserLogins);
                    context.UserClaims.RemoveRange(context.UserClaims);
                    context.UserTokens.RemoveRange(context.UserTokens);
                    context.UserRoles.RemoveRange(context.UserRoles);
                    context.Users.RemoveRange(context.Users);
                    context.SaveChanges();
                }
            }

            var response = Client.CreateAccountAsync().Result;
            var credentials = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
            Password = credentials.password;
            UserName = credentials.userName;
            
            var tokenResponse = Client.GenerateTokenAsync(new LoginModel
            {
                UserName = UserName,
                Password = Password
            }).Result;

            Token = JsonConvert.DeserializeObject<dynamic>(tokenResponse.Content.ReadAsStringAsync().Result).token;
        }
    }
}