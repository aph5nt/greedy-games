using Akka.Actor;
using Shared.Providers;
using WebApi.Configuration;

namespace WebApi.Providers.Impl
{
    public class RemoteTransactionManagerProvider : RemoteActorProvider, IRemoteTransactionManagerProvider
    {
        public override string Address => "/user/transaction";

        public RemoteTransactionManagerProvider(ActorSystem system, WebSettings configuration) 
            : base(system, configuration.AkkaSettings.AppServerUrl, configuration.AkkaSettings.ActorTimeout)
        {
        }
    }
}
 
 