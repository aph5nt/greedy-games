using Shared.Model;

namespace Payment.Messages.Commands.Dividends
{
    public class TriggerDividend
    {
        public Identity Identity { get; }

        public TriggerDividend(Identity identity)
        {
            Identity = identity;
        }
    }
}
