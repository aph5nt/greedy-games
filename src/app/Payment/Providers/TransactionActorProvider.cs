using Akka.Actor;
using Payment.Contracts.Providers;
using Shared.Providers;

namespace Payment.Providers
{
    public class TransactionActorProvider : ActorProvider, ITransactionManagerActorProvider
    {
        public TransactionActorProvider(IActorRefFactory system) : base(system)
        {
        }

        public override string Address => "user/transaction";
    }
}