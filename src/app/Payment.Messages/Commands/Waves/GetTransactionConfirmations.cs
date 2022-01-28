using System;
using Akka.Actor;
using Shared.Model;

namespace Payment.Messages.Commands.Waves
{
    public class GetTransactionConfirmations  : WavesCommand
    {
        public string TxSignature { get; private set; }

        public GetTransactionConfirmations(Network network, Object payload, IActorRef target, string txSignature) : base(network, payload, target)
        {
            TxSignature = txSignature;
        }
    }
}