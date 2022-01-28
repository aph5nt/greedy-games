using Akka.Actor;
using Payment.Contracts.Providers;
using Shared.Providers;

namespace Payment.Providers
{
    public class WavesActorProvider : ActorProvider, IWavesActorProvider
    {
        public WavesActorProvider(IActorRefFactory system) : base(system)
        {
        }

        public override string Address => "user/payment-gateway";
    }
}