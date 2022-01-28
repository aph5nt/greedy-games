using System.Net;
using Newtonsoft.Json;
using WebApi.Configuration;

namespace WebApi.Services.Impl
{
    public class ReCaptchaValidation : IReCaptchaValidation
    {
        private readonly WebSettings _settings;

        public ReCaptchaValidation(WebSettings settings)
        {
            _settings = settings;
        }

        public bool VerifyCaptcha(string token)
        {
            var requestUrl = @"https://www.google.com/recaptcha/api/siteverify";
            string secret = _settings.Recaptcha.SecretKey;
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.QueryString.Add("secret", secret);
                    wc.QueryString.Add("response", token);
                    var result = JsonConvert.DeserializeObject<ReCaptchaResponse>(wc.DownloadString(requestUrl));
                    return result.Success;
                }
            }
            catch (WebException)
            {
                return false;
            }
        }
    }
}