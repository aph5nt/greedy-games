using System;
using Akka.Actor;
using Shared.Model;

namespace Payment.Messages.Commands.Waves
{
    public class Transfer : WavesCommand
    {
        public long Amount { get; private set; }
        public long Fee { get; private set; }
        public string SourceAddress { get; private set; }
        public string TargetAddress { get; private set; }

        public Transfer(Network network, Object payload, IActorRef target, long amount, long fee, string sourceAddress, string targetAddress) : base(network, payload, target)
        {
            Amount = amount;
            Fee = fee;
            SourceAddress = sourceAddress;
            targetAddress = TargetAddress;
        }
    }
}