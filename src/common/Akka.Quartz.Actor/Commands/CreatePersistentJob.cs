using Akka.Actor;
using Quartz;

namespace Akka.Quartz.Actor.Commands
{
    /// <summary>
    ///     Message to add a trigger.
    /// </summary>
    public class CreatePersistentJob : IJobCommand
    {
        public CreatePersistentJob(ActorPath to, object message, ITrigger trigger)
        {
            To = to;
            Message = message;
            Trigger = trigger;
        }

        /// <summary>
        ///     The desination actor
        /// </summary>
        public ActorPath To { get; private set; }

        /// <summary>
        ///     Message
        /// </summary>
        public object Message { get; private set; }

        /// <summary>
        ///     Trigger 
        /// </summary>
        public ITrigger Trigger { get; private set; }
    }
}