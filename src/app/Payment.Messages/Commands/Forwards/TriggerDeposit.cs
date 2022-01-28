using Shared.Model;

namespace Payment.Messages.Commands.Forwards
{
    public class TriggerDeposit
    {
        public Identity Identity { get; }

        public TriggerDeposit(Identity identity)
        {
            Identity = identity;
        }
    }
}