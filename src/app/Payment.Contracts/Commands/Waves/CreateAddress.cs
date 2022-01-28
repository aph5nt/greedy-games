using System;
using Akka.Actor;
using Shared.Model;

namespace Payment.Contracts.Commands.Waves
{
    public class CreateAddress : WavesCommand
    {
        public CreateAddress(Network network, Object payload, IActorRef target) : base(network, payload, target)
        {
        }
    }
}