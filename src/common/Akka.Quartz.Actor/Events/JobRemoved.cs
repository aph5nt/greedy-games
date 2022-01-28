using Quartz;

namespace Akka.Quartz.Actor.Events
{
    /// <summary>
    ///     Job removed event
    /// </summary>
    public class JobRemoved : JobEvent
    {
        public JobRemoved(JobKey jobKey, TriggerKey triggerKey) : base(jobKey, triggerKey)
        {
        }


        public override string ToString()
        {
            return string.Format("{0} with trigger {1} has been removed.", JobKey, TriggerKey);
        }
    }
}