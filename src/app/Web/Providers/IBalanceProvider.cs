using Akka.Actor;
using Shared.Model;

namespace Web.Providers
{
    public interface IBalanceProvider
    {
        ActorSelection Provide(Network network);
    }
}