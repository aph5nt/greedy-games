using System;
using Akka.Actor;
using Shared.Model;

namespace Payment.Contracts.Commands.Balaces
{
    public abstract class BalanceCommand
    {
        public Network Network { get; }
        public string UserName { get; }
        public long Amount { get; }
        public long Fee { get; }
        public Object Payload { get; }
        public IActorRef Target { get; }
        
        public BalanceCommand(Network network, string userName, long amount, long fee, object payload, IActorRef target)
        {
            Network = network;
            UserName = userName;
            Amount = amount;
            Fee = fee;
            Payload = payload;
            Target = target;
        }
    }
}
