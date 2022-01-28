using Shared.Model;

namespace Payment.Contracts.Events.Withdraws
{
    public class DepositConfirmed : IDepositNotification
    {
        public Identity Identity { get; }

        public DepositConfirmed(Identity identity)
        {
            Identity = identity;
        }

        public DepositConfirmed(long id, Network network, string userName) : this(
            new Identity(id, network, userName))
        {
        }
    }
}