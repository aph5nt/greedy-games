using System;
using Akka.Actor;
using Shared.Model;

namespace Payment.Contracts.Commands.Waves
{
    public class GetTransactionConfirmations  : WavesCommand
    {
        public string TransactionId { get; private set; }

        public GetTransactionConfirmations(Network network, Object payload, IActorRef target, string txId) : base(network, payload, target)
        {
            TransactionId = txId;
        }
    }
}