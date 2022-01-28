using Akka.Actor;
using Shared.Providers;
using WebApi.Configuration;

namespace WebApi.Providers.Impl
{
    public class RemoteWavesGatewayProvider : RemoteActorProvider, IRemoteWavesGatewayProvider
    {
        public override string Address => "/user/payment-gateway";

        public RemoteWavesGatewayProvider(ActorSystem system, WebSettings configuration) 
            : base(system, configuration.AkkaSettings.AppServerUrl, configuration.AkkaSettings.ActorTimeout)
        {
        }
    }
}