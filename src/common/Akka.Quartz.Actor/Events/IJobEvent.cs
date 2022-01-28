using Quartz;

namespace Akka.Quartz.Actor.Events
{
    /// <summary>
    /// Base interface for job events
    /// </summary>
    internal interface IJobEvent
    {
        JobKey JobKey { get; }
        TriggerKey TriggerKey { get; }
    }
}