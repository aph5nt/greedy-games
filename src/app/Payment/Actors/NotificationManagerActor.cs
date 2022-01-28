using Akka.Actor;
using Payment.Contracts.Events.Withdraws;
using Payment.Contracts.Models;
using Shared.Configuration;
using Shared.Model;

namespace Payment.Actors
{
    public class NotificationManagerActor : ReceiveActor
    {
        private readonly AppServerSettings _settings;
        
        public NotificationManagerActor(AppServerSettings settings)
        {
            _settings = settings;
          
            Receive<Balance>(message =>
            {
                Context.Child("balance").Forward(message);
            }, balance =>
            {
                var isGameAccount = balance.UserName == GameTypes.Minefield.ToString() ||
                                    balance.UserName == GameTypes.ToTheMoon.ToString();
                
                if (balance.LogEventType.HasValue && !isGameAccount)
                {
                    if (balance.LogEventType.Value == LogEventType.Deposit ||
                        balance.LogEventType.Value == LogEventType.WithdrawLock ||
                        balance.LogEventType.Value == LogEventType.GameLock || 
                        balance.LogEventType.Value == LogEventType.ReleaseGameLock)
                    {
                        return true;
                    }
                }

                return false;
            });

            Receive<IDepositNotification>(message =>
            {
                Context.Child("deposit").Forward(message);
            });

            Receive<IWithdrawNotification>(message =>
            {
                Context.Child("withdraw").Forward(message);
            });
        }
        
        protected override void PreStart()
        {
            Context.ActorOf(Props.Create<HubNotificationActor<Balance>>(_settings, "balances"), "balance");
            Context.ActorOf(Props.Create<HubNotificationActor<IDepositNotification>>(_settings, "deposit"), "deposit");
            Context.ActorOf(Props.Create<HubNotificationActor<IWithdrawNotification>>(_settings, "withdraw"), "withdraw");
        }

        public static IActorRef Init(ActorSystem system, AppServerSettings settings)
        {
            var actor = system.ActorOf(Props.Create<NotificationManagerActor>(settings), "notifications");
            system.EventStream.Subscribe(actor, typeof(Balance));
            system.EventStream.Subscribe(actor, typeof(IDepositNotification));
            system.EventStream.Subscribe(actor, typeof(IWithdrawNotification));

            return actor;
        }
    }
}