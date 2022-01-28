using Akka.Actor;
using Akka.Configuration;
using Akka.Dispatch;
using Payment.Contracts.Models;

namespace Payment.Actors
{
    public class BalancePriorityMailbox : UnboundedPriorityMailbox
    {
        public BalancePriorityMailbox(Settings settings, Config config) : base(settings, config)
        {
        }
        
        protected override int PriorityGenerator(object message)
        {
            if (message is Balance)
            {
                return 0;
            }

            return 1;
        }
    }
}