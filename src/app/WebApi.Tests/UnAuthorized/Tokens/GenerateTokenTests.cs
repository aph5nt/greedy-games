using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using WebApi.Models;

namespace WebApi.Tests.UnAuthorized.Tokens
{
    [TestFixture]
    public class GenerateTokenTests : WebApiTest
    {
        [Test]
        public async Task ShouldSucceed()
        {
            var response = await this.Client.CreateAccountAsync();
            var credentials = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            string userName = credentials.userName;
            string password = credentials.password;

            var tokenResponse = await Client.GenerateTokenAsync(new LoginModel
            {
                UserName = userName,
                Password = password
            });

            var token = (string)JsonConvert.DeserializeObject<dynamic>(await tokenResponse.Content.ReadAsStringAsync()).token;

            Assert.True(tokenResponse.StatusCode == HttpStatusCode.OK);
            Assert.True(token.Length > 0);
        }
    }
}
