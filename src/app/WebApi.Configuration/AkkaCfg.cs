namespace WebApi.Configuration
{
    public class AkkaSettings
    {
        public int Port { get; set; }
        public string Hostname { get; set; }
        public string PublicHostname { get; set; }
        public string AppServerUrl { get; set; }
        public int ActorTimeout { get; set; }
    }
}