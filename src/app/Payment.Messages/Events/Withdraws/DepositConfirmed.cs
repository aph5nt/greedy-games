using Shared.Model;

namespace Payment.Messages.Events.Withdraws
{
    public class DepositConfirmed : IDepositNotification
    {
        public Identity Identity { get; private set; }

        public DepositConfirmed(Identity identity)
        {
            Identity = identity;
        }
    }
}