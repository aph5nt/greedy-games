using System;
using Akka.Actor;
using Shared.Model;

namespace Payment.Messages.Commands.Waves
{
    public class CreateAddress : WavesCommand
    {
        public CreateAddress(Network network, Object payload, IActorRef target) : base(network, payload, target)
        {
        }
    }
}