using System;
using Akka.Actor;
using Shared.Model;

namespace Payment.Contracts.Commands.Waves
{
    public class Transfer : WavesCommand
    {
        public long Amount { get;}
        public long Fee { get; }
        public string SourceAddress { get; }
        public string TargetAddress { get;  }

        public Transfer(Network network, Object payload, IActorRef target, long amount, long fee, string sourceAddress, string targetAddress) : base(network, payload, target)
        {
            Amount = amount;
            Fee = fee;
            SourceAddress = sourceAddress;
            TargetAddress = targetAddress;
        }
    }
}