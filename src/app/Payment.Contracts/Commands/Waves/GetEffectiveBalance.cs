using System;
using Akka.Actor;
using Shared.Model;

namespace Payment.Contracts.Commands.Waves
{
    public class GetEffectiveBalance : WavesCommand
    {
        public string Address { get; private set; }
     
        public GetEffectiveBalance(Network network, Object payload, IActorRef target, string address, long confirmations) :
            base(network, payload, target)
        {
            Address = address;
        }
    }
}