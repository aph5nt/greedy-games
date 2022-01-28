using Quartz;

namespace Akka.Quartz.Actor.Events
{
    /// <summary>
    ///     Job created event
    /// </summary>
    public class JobCreated : JobEvent
    {
        public JobCreated(JobKey jobKey, TriggerKey triggerKey) : base(jobKey, triggerKey)
        {
        }


        public override string ToString()
        {
            return string.Format("{0} with trigger {1} has been created.", JobKey, TriggerKey);
        }
    }
}