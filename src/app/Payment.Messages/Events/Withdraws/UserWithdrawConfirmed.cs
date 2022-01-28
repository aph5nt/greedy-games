using Shared.Model;

namespace Payment.Messages.Events.Withdraws
{
    public class UserWithdrawConfirmed : IWithdrawNotification
    {
        public Identity Identity { get; }

        public UserWithdrawConfirmed(Identity identity)
        {
            Identity = identity;
        }
    }
}