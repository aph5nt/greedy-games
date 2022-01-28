using Akka.Actor;
using Shared.Model;

namespace Payment.Contracts.Commands.Balaces
{
    public class VerifyBalance : BalanceCommand
    {
        public VerifyBalance(Network network, string userName, long amount, long fee, object payload, IActorRef target) : 
            base(network, userName, amount, fee, payload, target)
        {
            
        }
    }
}