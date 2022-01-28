using Shared.Model;

namespace Payment.Contracts.Events.Withdraws
{
    public class UserWithdrawFailed : IWithdrawNotification
    {
        public Identity Identity { get; }

        public UserWithdrawFailed(Identity identity)
        {
            Identity = identity;
        }
    }
}