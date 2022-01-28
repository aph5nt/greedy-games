using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Payment.Services.Impl
{
    public class ApiRequests : IDisposable
    {
        private const string ApplicationJson = "application/json";
        private readonly string _apiKey;
        private readonly HttpClient _client;
        
        public ApiRequests(Uri url, string apiKey)
        {
            _apiKey = apiKey;
            _client = new HttpClient
            {
                BaseAddress = url
            };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationJson));
        }
        
        public async Task<dynamic> PostRequest(string url, object body)
        {
            var bodyStr = JsonConvert.SerializeObject(body);
            var content = new StringContent(bodyStr);
            content.Headers.ContentType = new MediaTypeHeaderValue(ApplicationJson);

            var msg = new HttpRequestMessage(HttpMethod.Post, url);
            msg.Headers.Add("api_key", _apiKey);
            msg.Content = content;

            var response = await _client.SendAsync(msg, CancellationToken.None);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new WavesApiException(response.StatusCode, result);

            var json = JsonConvert.DeserializeObject<dynamic>(result);
            return json;
        }

        public async Task<dynamic> GetRequest(string url)
        {
            var msg = new HttpRequestMessage(HttpMethod.Get, url);
            msg.Headers.Add("api_key", _apiKey);

            var response = await _client.SendAsync(msg, CancellationToken.None);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new WavesApiException(response.StatusCode, result);

            var json = JsonConvert.DeserializeObject<dynamic>(result);

            return json;
        }
        
        public async Task<bool> DeleteAddressAsync(string url)
        {
            var content = new StringContent("{}");
            content.Headers.ContentType = new MediaTypeHeaderValue(ApplicationJson);

            var msg = new HttpRequestMessage(HttpMethod.Delete, url);
            msg.Headers.Add("api_key", _apiKey);
            msg.Content = content;

            var response = await _client.SendAsync(msg, CancellationToken.None);
            var result = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<dynamic>(result);

            return json.deleted;
        }
        
        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}