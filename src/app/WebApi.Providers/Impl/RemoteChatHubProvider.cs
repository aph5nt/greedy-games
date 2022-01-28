using Akka.Actor;
using Shared.Providers;
using WebApi.Configuration;

namespace WebApi.Providers.Impl
{
    public class RemoteChatHubProvider : RemoteActorProvider, IRemoteChatHubProvider
    {
        public sealed override string Address => "/user/chathub";

        public RemoteChatHubProvider(ActorSystem system, WebSettings configuration) :
            base(system, configuration.AkkaSettings.AppServerUrl, configuration.AkkaSettings.ActorTimeout)
        {
        }
    }
}