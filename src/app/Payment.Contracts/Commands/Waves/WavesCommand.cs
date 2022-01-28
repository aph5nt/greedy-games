using System;
using Akka.Actor;
using Shared.Model;

namespace Payment.Contracts.Commands.Waves
{
    public abstract class WavesCommand
    {
        public Object Payload { get; private set; }
        public IActorRef Target { get; private set; }
        public Network Network { get; private set; }

        public WavesCommand(Network network, Object payload, IActorRef target)
        {
            Network = network;
            Payload = payload;
            Target = target;
        }
    }
}