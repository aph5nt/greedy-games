using Akka.Actor;
using Web.Configuration;

namespace Web.Providers
{
    public class GameMinefieldProfider : ActorProvider, IGameMinefieldProfider
    {
        public GameMinefieldProfider(ActorSystem system, WebSettings configuration) : base(system, configuration)
        {
        }

        public override string Address => "/user/minefield";
    }
}
