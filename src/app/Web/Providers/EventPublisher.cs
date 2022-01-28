using Akka.Actor;
using Web.Configuration;

namespace Web.Providers
{
    public class EventPublisher : ActorProvider, IEventPublisher
    {
        public sealed override string Address => "/user/events";

        public EventPublisher(ActorSystem system, WebSettings configuration) :
            base(system, configuration)
        {
        }
    }
}