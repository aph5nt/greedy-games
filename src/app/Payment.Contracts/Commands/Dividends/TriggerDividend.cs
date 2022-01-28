using Shared.Model;

namespace Payment.Contracts.Commands.Dividends
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
