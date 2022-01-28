using Newtonsoft.Json;

namespace WebApi.Services
{
    public interface IReCaptchaValidation
    {
        bool VerifyCaptcha(string token);
    }

    public class ReCaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}