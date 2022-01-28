using Akka.Actor;
using Web.Configuration;

namespace Web.Providers
{
    public class WavesGatewayProvider : ActorProvider, IWavesGatewayProvider
    {
        public override string Address => "/user/payment-gateway";

        public WavesGatewayProvider(ActorSystem system, WebSettings configuration) : base(system, configuration)
        {
        }
    }
}