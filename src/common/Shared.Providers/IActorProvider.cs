using Akka.Actor;

namespace Shared.Providers
{
    public interface IActorProvider
    {
        IActorRef Provide();
    }
}