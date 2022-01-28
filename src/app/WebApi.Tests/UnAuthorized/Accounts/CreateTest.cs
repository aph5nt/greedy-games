using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

namespace WebApi.Tests.UnAuthorized.Accounts
{
    [TestFixture]
    public class CreateTests : WebApiTest
    {
        [Test]
        public async Task ShouldSucceed()
        {
            var response = await this.Client.CreateAccountAsync();
            var credentials = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            string userName = credentials.userName;
            string password = credentials.password;

            Assert.True(response.StatusCode == HttpStatusCode.Created);
            Assert.True(userName.Length > 0);
            Assert.True(password.Length > 0);
        }
    }
}
