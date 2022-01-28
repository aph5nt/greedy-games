using Akka.Actor;
using Akka.DI.Core;
using Akka.Quartz.Actor;
using Akka.Quartz.Actor.Commands;
using Payment.Contracts.Commands.Dividends;
using Payment.Contracts.Events.Dividends;
using Payment.Contracts.Events.Withdraws;
using Quartz;
using Shared.Model;
using System;
using Akka.Actor.Dsl;


/*
 
    UserWithdrawConfirmationActor -- > JOB --> triggered by sytem on event

DepositJob --> triggered by system on event
DepositJobConfirmation --> triggered by system on event

DividendSchedulerActor --> fixed JOB
DividendSchedulerActorCOnfirmation --> same story as in withdraw
     
     */
namespace Payment.Actors.Jobs
{
    
    
    
    
    
    public class DividendSchedulerActor : TypedActor,
        IHandle<TriggerDividend>,
        IHandle<DividendWithdrawed>,
        IHandle<DividendConfirmed>,
        IHandle<DividendFailed>
    {
        private IActorRef _quartzActor;
        private IActorRef _dividendActor;
        private IActorRef _dividendConfirmationActor;

        private readonly Quartz.IScheduler _scheduler;

        public DividendSchedulerActor(Quartz.IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public static void Init(ActorSystem system, Quartz.IScheduler scheduler)
        {
            var receiver = system.ActorOf(Props.Create<DividendSchedulerActor>(scheduler), "dividend-scheduler");
            system.EventStream.Subscribe(receiver, typeof(DividendWithdrawed));
            system.EventStream.Subscribe(receiver, typeof(DividendConfirmed));
            system.EventStream.Subscribe(receiver, typeof(DividendFailed));
        }

        protected override void PreStart()
        {
            _quartzActor = Context.ActorOf(Props.Create(() => new QuartzPersistentActor(_scheduler)), "scheduler");
            _dividendActor = Context.ActorOf(Context.DI().Props<DividendActor>(), "dividend");
            _dividendConfirmationActor = Context.ActorOf(Context.DI().Props<DividendConfirmationActor>(), "confirm");

            Self.Tell(new TriggerDividend(new Identity(Network.WAVES, GameTypes.Minefield.ToString())));
            Self.Tell(new TriggerDividend(new Identity(Network.GREEDYTEST, GameTypes.Minefield.ToString())));
            
            base.PreStart();
        }

        public void Handle(TriggerDividend message)
        {
            var dividendTrigger =
                     TriggerBuilder.Create()
                     .WithIdentity(Name(message), "dividends")
                     .WithCronSchedule("0 0 12 ? * FRI")
                     .Build();

            if (!_scheduler.CheckExists(dividendTrigger.Key).GetAwaiter().GetResult())
            {
                _quartzActor.Tell(new CreatePersistentJob(_dividendActor.Path, message, dividendTrigger));
            }
        }

        public void Handle(DividendWithdrawed message)
        {
            var depositWithdrawedTrigger =
                     TriggerBuilder.Create()
                     .WithIdentity(Name(message), "dividends")
                     .StartAt(DateBuilder.NextGivenSecondDate(DateTime.UtcNow, 10))
                     .WithSimpleSchedule(x => x.WithIntervalInMinutes(6).RepeatForever())
                                         .Build();

            if (!_scheduler.CheckExists(depositWithdrawedTrigger.Key).GetAwaiter().GetResult())
            {
                _quartzActor.Tell(new CreatePersistentJob(_dividendConfirmationActor.Path, message, depositWithdrawedTrigger));
            }
        }

        public void Handle(DividendConfirmed message)
        {
            var triggerKey = new TriggerKey(Name(message), "dividends");
            var jobKey = new JobKey(Name(message), "dividends");
            _quartzActor.Tell(new RemoveJob(jobKey, triggerKey));
        }

        public void Handle(DividendFailed message)
        {
            var triggerKey = new TriggerKey(Name(message), "dividends");
            var jobKey = new JobKey(Name(message), "dividends");
            _quartzActor.Tell(new RemoveJob(jobKey, triggerKey));
        }

        private static string Name(object message)
        {
            switch (message)
            {
                case TriggerDividend msg:
                    return $"{nameof(TriggerDividend)}-{msg.Identity.Network}-{msg.Identity.UserName}";

                case DividendWithdrawed msg:
                    return $"{nameof(DividendWithdrawed)}-{msg.Withdraw.Network}-{msg.Withdraw.GameName}";
                case DividendConfirmed msg:
                    return $"{nameof(DividendWithdrawed)}-{msg.Withdraw.Network}-{msg.Withdraw.GameName}";
                case DividendFailed msg:
                    return $"{nameof(DividendWithdrawed)}-{msg.Withdraw.Network}-{msg.Withdraw.GameName}";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}