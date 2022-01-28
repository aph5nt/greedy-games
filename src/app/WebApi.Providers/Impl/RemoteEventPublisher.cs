using Akka.Actor;
using Shared.Providers;
using WebApi.Configuration;

namespace WebApi.Providers.Impl
{
    public class RemoteEventPublisher : RemoteActorProvider, IRemoteEventPublisher
    {
        public sealed override string Address => "/user/events";

        public RemoteEventPublisher(ActorSystem system, WebSettings configuration) :
            base(system, configuration.AkkaSettings.AppServerUrl, configuration.AkkaSettings.ActorTimeout)
        {
        }
    }
}