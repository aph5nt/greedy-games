using Shared.Model;

namespace Payment.Messages.Events.Withdraws
{
    public class DepositFailed : IDepositNotification
    {
        public Identity Identity { get; private set; }

        public DepositFailed(Identity identity)
        {
            Identity = identity;
        }
    }
}