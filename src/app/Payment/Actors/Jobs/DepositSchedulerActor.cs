using Akka.Actor;
using Akka.DI.Core;
using Akka.Quartz.Actor;
using Akka.Quartz.Actor.Commands;
using Akka.Routing;
using Payment.Contracts.Commands.Forwards;
using Payment.Contracts.Events.Forwards;
using Payment.Contracts.Events.Withdraws;
using Quartz;
using System;

namespace Payment.Actors.Jobs
{
    public class DepositSchedulerActor : TypedActor,
        IHandle<TriggerDeposit>,
        IHandle<DepositPlaced>,
        IHandle<DepositConfirmed>,
        IHandle<DepositFailed>
    {
        private IActorRef _quartzActor;
        private IActorRef _depositActor;
        private IActorRef _depositConfirmationActor;
 
        private readonly Quartz.IScheduler _scheduler;

        public DepositSchedulerActor(Quartz.IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public static void Init(ActorSystem system, Quartz.IScheduler scheduler)
        {
            var receiver = system.ActorOf(Props.Create<DepositSchedulerActor>(scheduler), "deposit-scheduler");
            system.EventStream.Subscribe(receiver, typeof(TriggerDeposit));
            system.EventStream.Subscribe(receiver, typeof(DepositPlaced));
            system.EventStream.Subscribe(receiver, typeof(DepositConfirmed));
            system.EventStream.Subscribe(receiver, typeof(DepositFailed));
        }

        protected override void PreStart()
        {
            _quartzActor = Context.ActorOf(Props.Create(() => new QuartzPersistentActor(_scheduler)), "scheduler");
            _depositActor = Context.ActorOf(Context.DI().Props<DepositActor>().WithRouter(new RoundRobinPool(1, new DefaultResizer(1, 10))), "deposit");
            _depositConfirmationActor = Context.ActorOf(Context.DI().Props<DepositConfirmationActor>().WithRouter(new RoundRobinPool(1, new DefaultResizer(1, 10))), "confirm");

            base.PreStart();
        }

        public void Handle(TriggerDeposit message)
        {
            var userLoggedTrigger =
                     TriggerBuilder.Create()
                     .WithIdentity(Name(message), "deposits")
                     .StartAt(DateBuilder.NextGivenSecondDate(DateTime.UtcNow, 10))
                     .WithSimpleSchedule(x => x.WithIntervalInMinutes(10).WithRepeatCount(36))
                                         .Build();

            if (!_scheduler.CheckExists(userLoggedTrigger.Key).GetAwaiter().GetResult())
            {
                _quartzActor.Tell(new CreatePersistentJob(_depositActor.Path, message, userLoggedTrigger));
            }
        }

        public void Handle(DepositPlaced message)
        {
            var depositPlacedTrigger =
                     TriggerBuilder.Create()
                     .WithIdentity(Name(message), "deposits")
                     .StartAt(DateBuilder.NextGivenSecondDate(DateTime.UtcNow, 10))
                     .WithSimpleSchedule(x => x.WithIntervalInMinutes(6).RepeatForever())
                                         .Build();

            if (!_scheduler.CheckExists(depositPlacedTrigger.Key).GetAwaiter().GetResult())
            {
                _quartzActor.Tell(new CreatePersistentJob(_depositConfirmationActor.Path, message, depositPlacedTrigger));
            }
        }

        public void Handle(DepositConfirmed message)
        {
            var triggerKey = new TriggerKey(Name(message), "deposits");
            var jobKey = new JobKey(Name(message), "deposits");
            _quartzActor.Tell(new RemoveJob(jobKey, triggerKey));
        }
        public void Handle(DepositFailed message)
        {
            var triggerKey = new TriggerKey(Name(message), "deposits");
            var jobKey = new JobKey(Name(message), "deposits");
            _quartzActor.Tell(new RemoveJob(jobKey, triggerKey));
        }

        public static string Name(object message)
        {
            switch (message)
            {
                case TriggerDeposit msg:
                    return $"{nameof(TriggerDeposit)}-{msg.Identity.Network}-{msg.Identity.UserName}";

                case DepositPlaced msg:
                    return $"{nameof(DepositPlaced)}-{msg.Deposit.Network}-{msg.Deposit.UserName}";
                case DepositConfirmed msg:
                    return $"{nameof(DepositPlaced)}-{msg.Identity.Network}-{msg.Identity.UserName}";
                case DepositFailed msg:
                    return $"{nameof(DepositPlaced)}-{msg.Identity.Network}-{msg.Identity.UserName}";
                default:
                    throw new NotSupportedException();
            }
        }
 
    }
}