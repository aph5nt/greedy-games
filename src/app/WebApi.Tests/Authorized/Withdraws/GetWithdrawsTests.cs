using Newtonsoft.Json;
using NUnit.Framework;
using Shared.Model;
using System.Net;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Tests.Authorized.Withdraws
{
    [TestFixture, Order(1)]
    public class GetWithdrawsTests : WebApiTest
    {
        [Order(1)]
       
        [TestCase(Network.WAVES)]
        [TestCase(Network.GREEDYTEST)]
        public async Task Should_Return_Deposits(Network network)
        {
            await Client.ActivateNetwork(AuthorizedSetup.Token, network);
            var response = await Client.GetWithdraws(AuthorizedSetup.Token, network);
            var result = JsonConvert.DeserializeObject<WithdrawModel>(await response.Content.ReadAsStringAsync());

            Assert.True(response.StatusCode == HttpStatusCode.OK);
            Assert.True(result.Withdraws.Count == 0);
            Assert.True(result.UserAccount.IsActive);
            Assert.True(result.UserAccount.UserName == AuthorizedSetup.UserName);
            Assert.True(result.UserAccount.DepositAddress.Length >= 0);
        }

        [Order(2)]
        public async Task Should_Return_BadRequest_For_FREE_Network()
        {
            var response = await Client.GetDeposits(AuthorizedSetup.Token, Network.FREE);
            Assert.True((int)response.StatusCode == 422);
        }
    }
}
