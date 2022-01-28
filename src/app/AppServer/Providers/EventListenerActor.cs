using Akka.Actor;

namespace AppServer.Providers
{
    public class EventListenerActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            Context.System.EventStream.Publish(message);
        }
    }
}