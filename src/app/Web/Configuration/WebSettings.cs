namespace Web.Configuration
{
    public class WebSettings
    {
        public string ConnectionString { get; set; }
        public string Storage { get; set; }
        public Recaptcha Recaptcha { get; set; }
        public AkkaCfg AkkaCfg { get; set; }
        public Notifications Notifications { get; set; }
        public Session Session { get; set; }
        public Tokens Tokens { get; set; }
    }
}
