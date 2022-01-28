using Akka.Actor;
using Akka.Quartz.Actor.Commands;
using Akka.Quartz.Actor.Events;
using Akka.Quartz.Actor.Exceptions;
using Quartz.Impl;
using System;
using System.Collections.Specialized;
using IScheduler = Quartz.IScheduler;

namespace Akka.Quartz.Actor
{
    /// <summary>
    /// The base quartz scheduling actor. Handles a single quartz scheduler
    /// and processes Add and Remove messages.
    /// </summary>
    public class QuartzActor : ActorBase
    {
        protected readonly IScheduler Scheduler;

        private readonly bool _externallySupplied;

        public QuartzActor()
        {
            Scheduler = new StdSchedulerFactory().GetScheduler().GetAwaiter().GetResult();
        }

        public QuartzActor(NameValueCollection props)
        {
            Scheduler = new StdSchedulerFactory(props).GetScheduler().GetAwaiter().GetResult();
        }

        public QuartzActor(IScheduler scheduler)
        {
            Scheduler = scheduler;
            _externallySupplied = true;
        }

        protected override bool Receive(object message)
        {
            return message.Match().With<CreateJob>(CreateJobCommand).With<RemoveJob>(RemoveJobCommand).WasHandled;
        }

        protected override void PreStart()
        {
            if (!_externallySupplied)
            {
                Scheduler.Start().GetAwaiter().GetResult();
            }
            
            base.PreStart();
        }

        protected override void PostStop()
        {
            if (!_externallySupplied)
            {
                Scheduler.Shutdown().GetAwaiter().GetResult();
            }
            
            base.PostStop();
        }

        protected virtual void CreateJobCommand(CreateJob createJob)
        {
            if (createJob.To == null)
            {
                Context.Sender.Tell(new CreateJobFail(null, null, new ArgumentNullException(nameof(createJob.To))));
            }
            if (createJob.Trigger == null)
            {
                Context.Sender.Tell(new CreateJobFail(null, null, new ArgumentNullException(nameof(createJob.Trigger))));
            }
            else
            {

                try
                {
                    var job =
                        QuartzJob.CreateBuilderWithData(createJob.To, createJob.Message)
                            .WithIdentity(createJob.Trigger.Key.Name, createJob.Trigger.Key.Group)
                            .StoreDurably(true)
                            .Build();
                    
                    Scheduler.ScheduleJob(job, createJob.Trigger).GetAwaiter();

                    Context.Sender.Tell(new JobCreated(createJob.Trigger.JobKey, createJob.Trigger.Key));
                }
                catch (Exception ex)
                {
                    Context.Sender.Tell(new CreateJobFail(createJob.Trigger.JobKey, createJob.Trigger.Key, ex));
                }
            }
        }

        protected virtual void RemoveJobCommand(RemoveJob removeJob)
        {
            try
            {
                var deleted = Scheduler.DeleteJob(removeJob.JobKey).GetAwaiter().GetResult();
                if (deleted)
                {
                    Context.Sender.Tell(new JobRemoved(removeJob.JobKey, removeJob.TriggerKey));
                }
                else
                {
                    Context.Sender.Tell(new RemoveJobFail(removeJob.JobKey, removeJob.TriggerKey, new JobNotFoundException()));
                }
            }
            catch (Exception ex)
            {
                Context.Sender.Tell(new RemoveJobFail(removeJob.JobKey, removeJob.TriggerKey, ex));
            }
        }
    }
}