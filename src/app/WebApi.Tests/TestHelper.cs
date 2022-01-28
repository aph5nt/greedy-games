using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Model;
using WebApi.Models;

namespace WebApi.Tests
{
    public static class TestHelper
    {
        public static async Task<HttpResponseMessage> CreateAccountAsync(this HttpClient client)
        {
            return await client.PostAsync("api/account", GetStringContent(null));
        }

        public static async Task<HttpResponseMessage> GenerateTokenAsync(this HttpClient client, LoginModel model)
        {
            return await client.PostAsync("api/token", GetStringContent(model));
        }

        public static async Task<HttpResponseMessage> GetDeposits(this HttpClient client, string token, Network network)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            return await client.GetAsync($"api/deposit/{network.ToString().ToLower()}");
        }

        public static async Task<HttpResponseMessage> GetWithdraws(this HttpClient client, string token, Network network)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            return await client.GetAsync($"api/withdraw/{network.ToString().ToLower()}");
        }

        public static async Task<HttpResponseMessage> Withdraw(this HttpClient client, string token, WithdrawFormModel model)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            return await client.PostAsync("api/withdraw", GetStringContent(model));
        }

        public static async Task<HttpResponseMessage> ActivateNetwork(this HttpClient client, string token, Network network)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            return await client.PutAsync($"api/deposit/{network.ToString().ToLower()}", null);
        }

        public static StringContent GetStringContent(object body)
        {
            var content = new StringContent(JsonConvert.SerializeObject(body));
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            return content;
        }
    }
}