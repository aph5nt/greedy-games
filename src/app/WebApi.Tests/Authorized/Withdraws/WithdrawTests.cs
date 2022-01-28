using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NUnit.Framework;
using Shared.Model;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Tests.Authorized.Withdraws
{
    public class WithdrawTests : WebApiTest
    {
        [Test]
        public async Task Should_Return_UnprocessableEntity_For_EmptyBody()
        {
            var response = await Client.Withdraw(AuthorizedSetup.Token, new WithdrawFormModel());

            Assert.True((int)response.StatusCode == StatusCodes.Status422UnprocessableEntity);
            var result = JsonConvert.DeserializeObject<ValidationErrors>(await response.Content.ReadAsStringAsync());

            Assert.True(result.Errors.Any(x => x.Name.ToLower() == nameof(WithdrawFormModel.Network).ToLower()));
            Assert.True(result.Errors.Any(x => x.Name.ToLower() == nameof(WithdrawFormModel.DestinationAddress).ToLower()));
            Assert.True(result.Errors.Any(x => x.Name.ToLower() == nameof(WithdrawFormModel.Password).ToLower()));
        }

        [Test]
        public async Task Should_Return_UnprocessableEntity_For_InvalidWavesAddress()
        {
            var response = await Client.Withdraw(AuthorizedSetup.Token, new WithdrawFormModel
            {
                DestinationAddress = "invalid waves address",
                Amount = 10 * Money.Sathoshi,
                Network = Network.GREEDYTEST,
                Password = AuthorizedSetup.Password
            });

            Assert.True((int)response.StatusCode == StatusCodes.Status422UnprocessableEntity);
            var result = JsonConvert.DeserializeObject<ValidationErrors>(await response.Content.ReadAsStringAsync());
            Assert.True(result.Errors.All(x => x.Name.ToLower() == nameof(WithdrawFormModel.DestinationAddress).ToLower()));
        }

        [Test]
        public async Task Should_Return_UnprocessableEntity_For_FREE_Network()
        {
            var response = await Client.Withdraw(AuthorizedSetup.Token, new WithdrawFormModel
            {
                DestinationAddress = "3PE5ZLLSdrryLZ6TSCJZhimY9HxmQT8m4Ty",
                Amount = 8 * Money.Sathoshi,
                Network = Network.FREE,
                Password = AuthorizedSetup.Password
            });

            Assert.True((int)response.StatusCode == StatusCodes.Status422UnprocessableEntity);
            var result = JsonConvert.DeserializeObject<ValidationErrors>(await response.Content.ReadAsStringAsync());
            Assert.True(result.Errors.All(x => x.Name.ToLower() == nameof(WithdrawFormModel.Network).ToLower()));
        }

        [TestCase(1000000L)]
        [TestCase(100000000000000000L)]
        public async Task Should_Return_UnprocessableEntity_For_InvalidAmount(long amount)
        {
            var response = await Client.Withdraw(AuthorizedSetup.Token, new WithdrawFormModel
            {
                DestinationAddress = "3PE5ZLLSdrryLZ6TSCJZhimY9HxmQT8m4Ty",
                Amount = amount * Money.Sathoshi,
                Network = Network.GREEDYTEST,
                Password = AuthorizedSetup.Password
            });

            Assert.True((int)response.StatusCode == StatusCodes.Status422UnprocessableEntity);
            var result = JsonConvert.DeserializeObject<ValidationErrors>(await response.Content.ReadAsStringAsync());
            Assert.True(result.Errors.All(x => x.Name.ToLower() == nameof(WithdrawFormModel.Amount).ToLower()));
        }

        [Test]
        public async Task Should_Return_UnAuthorized_For_InvalidPassword()
        {
            var response = await Client.Withdraw(AuthorizedSetup.Token, new WithdrawFormModel
            {
                DestinationAddress = "3PE5ZLLSdrryLZ6TSCJZhimY9HxmQT8m4Ty",
                Amount = 5 * Money.Sathoshi,
                Network = Network.GREEDYTEST,
                Password ="invalid password"
            });

            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Should_Return_UnprocessableEntity_For_No_Funds()
        {
            var response = await Client.Withdraw(AuthorizedSetup.Token, new WithdrawFormModel
            {
                DestinationAddress = "3PE5ZLLSdrryLZ6TSCJZhimY9HxmQT8m4Ty",
                Network = Network.GREEDYTEST,
                Password = AuthorizedSetup.Password,
                Amount = 10 * Money.Sathoshi
            });

            Assert.True((int)response.StatusCode == 422);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ValidationErrors>(content);

            Assert.True(result.Errors.Exists(x => x.Name.ToLower() == nameof(WithdrawFormModel.Amount).ToLower()));
        }
    }
}