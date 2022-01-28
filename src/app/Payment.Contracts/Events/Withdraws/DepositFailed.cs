using Shared.Model;

namespace Payment.Contracts.Events.Withdraws
{
    public class DepositFailed : IDepositNotification
    {
        public Identity Identity { get; private set; }

        public DepositFailed(Identity identity)
        {
            Identity = identity;
        }

        public DepositFailed(long id, Network network, string userName) : this(
            new Identity(id, network, userName))
        {
        }
    }
}