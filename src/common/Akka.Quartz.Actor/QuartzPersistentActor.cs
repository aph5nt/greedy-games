using System;
using Akka.Actor;
using Akka.Quartz.Actor.Commands;
using Akka.Quartz.Actor.Events;
using Quartz;
using IScheduler = Quartz.IScheduler;

namespace Akka.Quartz.Actor
{
    /// <summary>
    /// The persistent quartz scheduling actor. Handles a single quartz scheduler
    /// and processes CreatePersistentJob and RemoveJob messages.
    /// </summary>
    [DisallowConcurrentExecution]
    public class QuartzPersistentActor : QuartzActor
    {
        public QuartzPersistentActor(IScheduler scheduler)
            : base(scheduler)
        {
            AddSystemToScheduler();
        }

        private void AddSystemToScheduler()
        {
            if (!Scheduler.Context.ContainsKey(QuartzPersistentJob.SysKey))
            {
                Scheduler.Context.Add(QuartzPersistentJob.SysKey, Context.System);
            }
            else
            {
                Scheduler.Context.Remove(QuartzPersistentJob.SysKey);
                Scheduler.Context.Add(QuartzPersistentJob.SysKey, Context.System);
            }
        }

        protected override bool Receive(object message)
        {
            return message.Match().With<CreatePersistentJob>(CreateJobCommand).With<RemoveJob>(RemoveJobCommand).WasHandled;
        }

        private void CreateJobCommand(CreatePersistentJob createJob)
        {
            if (createJob.To == null)
            {
                Context.Sender.Tell(new CreateJobFail(null, null, new ArgumentNullException("createJob.To")));
            }
            if (createJob.Trigger == null)
            {
                Context.Sender.Tell(new CreateJobFail(null, null, new ArgumentNullException("createJob.Trigger")));
            }
            else
            {

                try
                {
                    var job =
                    QuartzPersistentJob.CreateBuilderWithData(createJob.To, createJob.Message, Context.System)
                        .WithIdentity(createJob.Trigger.Key.Name, createJob.Trigger.Key.Group)
                        .StoreDurably(true)
                        .Build();
                    
                    Scheduler.ScheduleJob(job, createJob.Trigger).GetAwaiter().GetResult();

                    Context.Sender.Tell(new JobCreated(createJob.Trigger.JobKey, createJob.Trigger.Key));
                }
                catch (Exception ex)
                {
                    Context.Sender.Tell(new CreateJobFail(createJob.Trigger.JobKey, createJob.Trigger.Key, ex));
                }
            }
        }
    }
}
