using Akka.Actor;

namespace Web.Providers
{
    public interface IActorSelectionProvider
    {
        string Address { get; }
        IActorRef Provide();
    }
}
