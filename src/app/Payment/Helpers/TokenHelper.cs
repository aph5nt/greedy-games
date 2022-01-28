using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Shared.Configuration;

namespace Payment.Helpers
{
    public static class TokenHelper
    {
        public static HttpResponseMessage GetToken(AppServerSettings settings)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(settings.Notifications.Url);
                    
                    var content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        userName = settings.Notifications.UserName,
                        password = settings.Notifications.UserPassword
                    }));
                    
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                    return client.PostAsync("api/token", content).Result;
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}