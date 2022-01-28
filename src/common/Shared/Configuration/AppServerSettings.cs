using System;

namespace Shared.Configuration
{
    public class AppServerSettings
    {
        public ConnectionString ConnectionString { get; set; }
        public Waves Waves { get; set; }
        public Payments Payments { get; set; }
        public AkkaSettings AkkaSettings { get; set; }
        public Notifications Notifications { get; set; }
    }
}