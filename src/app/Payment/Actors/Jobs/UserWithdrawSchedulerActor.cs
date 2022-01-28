using Akka.Actor;
using Akka.DI.Core;
using Akka.Quartz.Actor;
using Akka.Quartz.Actor.Commands;
using Payment.Contracts.DataTransfer;
using Payment.Contracts.Events.Withdraws;
using Persistance.Model.Payments;
using Quartz;

namespace Payment.Actors.Jobs
{
    public class UserWithdrawSchedulerActor : TypedActor,
        IHandle<UserMoneyWithdrawed>,
        IHandle<UserWithdrawConfirmed>,
        IHandle<UserWithdrawFailed>
    {
        private readonly Quartz.IScheduler _scheduler;
        private IActorRef _quartzActor;
        private IActorRef _userWithdrawConfirmationActor;

        public static void Init(ActorSystem system, Quartz.IScheduler scheduler)
        {
            var receiver = system.ActorOf(Props.Create(() => new UserWithdrawSchedulerActor(scheduler)), "userWithdrawScheduler");
            system.EventStream.Subscribe(receiver, typeof(UserMoneyWithdrawed));
            system.EventStream.Subscribe(receiver, typeof(UserWithdrawConfirmed));
            system.EventStream.Subscribe(receiver, typeof(UserWithdrawFailed));
        }

        public UserWithdrawSchedulerActor(Quartz.IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        protected override void PreStart()
        {
            _quartzActor = Context.ActorOf(Props.Create(() => new QuartzPersistentActor(_scheduler)), "quartz");
            _userWithdrawConfirmationActor = Context.ActorOf(Context.DI().Props<UserWithdrawConfirmationActor>(), "confirm");

            base.PreStart();
        }

        public void Handle(UserMoneyWithdrawed message)
        {
            var userWithdrawTrigger =
                         TriggerBuilder.Create()
                         .WithIdentity(Name(message.Payload), "withdraws")
                         .StartAt(DateBuilder.NextGivenSecondDate(System.DateTime.UtcNow, 10))
                         .WithSimpleSchedule(x => x.WithIntervalInMinutes(10).RepeatForever())
                                             .Build();

            if (!_scheduler.CheckExists(userWithdrawTrigger.Key).GetAwaiter().GetResult())
            {
                _quartzActor.Tell(new CreatePersistentJob(_userWithdrawConfirmationActor.Path, message, userWithdrawTrigger));
            }
        }

        public void Handle(UserWithdrawConfirmed message)
        {
            var triggerKey = new TriggerKey(Name(message), "withdraws");
            var jobKey = new JobKey(Name(message), "withdraws");
            _quartzActor.Tell(new RemoveJob(jobKey, triggerKey));
        }

        public void Handle(UserWithdrawFailed message)
        {
            var triggerKey = new TriggerKey(Name(message), "withdraws");
            var jobKey = new JobKey(Name(message), "withdraws");
            _quartzActor.Tell(new RemoveJob(jobKey, triggerKey));
        }

        private static string Name(UserWithdrawDto withdraw)
        {
            return $"{nameof(UserWithdrawDto)}-{withdraw.Network}-{withdraw.Id}";
        }
        private static string Name(UserWithdrawConfirmed @event)
        {
            return $"{nameof(UserWithdrawDto)}-{@event.Identity.Network}-{@event.Identity.Id}";
        }
        private static string Name(UserWithdrawFailed @event)
        {
            return $"{nameof(UserWithdraw)}-{@event.Identity.Network}-{@event.Identity.Id}";
        }
    }
}