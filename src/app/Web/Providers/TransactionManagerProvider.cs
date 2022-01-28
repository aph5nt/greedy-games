using Akka.Actor;
using Web.Configuration;

namespace Web.Providers
{
    public class TransactionManagerProvider : ActorProvider, ITransactionManagerProvider
    {
        public override string Address => "/user/transaction";

        public TransactionManagerProvider(ActorSystem system, WebSettings configuration) : base(system, configuration)
        {
        }
    }
}
 
 