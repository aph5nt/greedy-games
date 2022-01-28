using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Persistance.Model.Accounts;
using Shared.Model;

namespace WebApi.Tests.Authorized.Deposits
{
    [TestFixture, Order(2)]
    public class ActivateNetworkTests : WebApiTest
    {
        [Order(1)]
        [TestCase(Network.WAVES)]
        [TestCase(Network.GREEDYTEST)]
        public async Task ShouldSuccess(Network network)
        {
            var response = await Client.ActivateNetwork(AuthorizedSetup.Token, network);
            Assert.True(response.StatusCode == HttpStatusCode.NoContent);

            var userAccount = SetUp.GetDataContext().Accounts.OfType<UserAccount>()
                .Single(x => x.UserName == AuthorizedSetup.UserName && x.Network == network);

            Assert.True(userAccount.IsActive);
            Assert.True(userAccount.DepositAddress.Length > 0);
        }

        [Test, Order(2)]
        public async Task Should_Not_Activate_FREE_Account()
        {
            var response = await Client.ActivateNetwork(AuthorizedSetup.Token, Network.FREE);
            Assert.True(response.StatusCode == HttpStatusCode.Forbidden);
        }
    }
}