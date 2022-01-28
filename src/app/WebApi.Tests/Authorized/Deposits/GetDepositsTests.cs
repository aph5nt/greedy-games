using Newtonsoft.Json;
using NUnit.Framework;
using Shared.Model;
using System.Net;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Tests.Authorized.Deposits
{
    [TestFixture, Order(1)]
    public class GetDepositsTests : WebApiTest
    {
        [Order(1)]
        [TestCase(Network.WAVES)]
        [TestCase(Network.GREEDYTEST)]
        public async Task Should_Return_Deposits(Network network)
        {
            var response = await Client.GetDeposits(AuthorizedSetup.Token, network);
            var result = JsonConvert.DeserializeObject<DepositViewModel>(await response.Content.ReadAsStringAsync());

            Assert.True(response.StatusCode == HttpStatusCode.OK);
            Assert.True(result.Deposits.Count == 0);
            Assert.True(result.UserAccount.IsActive == false);
            Assert.True(result.UserAccount.UserName == AuthorizedSetup.UserName);
            Assert.True(result.UserAccount.DepositAddress.Length == 0);
        }

        [Order(2)]
        public async Task Should_Return_BadRequest_For_FREE_Network()
        {
            var response = await Client.GetDeposits(AuthorizedSetup.Token, Network.FREE);
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }
    }
}
