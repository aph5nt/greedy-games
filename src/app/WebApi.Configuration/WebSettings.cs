namespace WebApi.Configuration
{
    public class WebSettings
    {
        public string ConnectionString { get; set; }
        public string Storage { get; set; }
        public Recaptcha Recaptcha { get; set; }
        public AkkaSettings AkkaSettings { get; set; }
        public Notifications Notifications { get; set; }
        public Session Session { get; set; }
        public Tokens Tokens { get; set; }
        public Withdraw Withdraw { get; set; }
    }
}

