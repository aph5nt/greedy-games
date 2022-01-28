using Akka.Actor;
using Shared.Providers;
using WebApi.Configuration;

namespace WebApi.Providers.Impl
{
    public class RemoteGameMinefieldProfider : RemoteActorProvider, IRemoteGameMinefieldProfider
    {
        public RemoteGameMinefieldProfider(ActorSystem system, WebSettings configuration) 
            : base(system, configuration.AkkaSettings.AppServerUrl, configuration.AkkaSettings.ActorTimeout)
        {
        }

        public override string Address => "/user/minefield";
    }
}
