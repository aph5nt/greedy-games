using System;
using Akka.Actor;
using Shared.Model;

namespace Payment.Messages.Commands.Waves
{
    public class GetEffectiveBalance : WavesCommand
    {
        public string Address { get; private set; }
        public long Confirmations { get; private set; }

        public GetEffectiveBalance(Network network, Object payload, IActorRef target, string address, long confirmations) : base(network, payload, target)
        {
            Address = address;
            Confirmations = confirmations;
        }
    }
}