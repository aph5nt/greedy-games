namespace WebApi.Services.Impl
{
    public class TrueReCaptchaValidation : IReCaptchaValidation
    {
        public bool VerifyCaptcha(string token)
        {
            return true;
        }
    }
}