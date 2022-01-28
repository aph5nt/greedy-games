using Akka.Actor;
using Shared.Providers;
using WebApi.Configuration;

namespace WebApi.Providers.Impl
{
    public class RemoteUserWithdrawProvider : RemoteActorProvider, IRemoteUserWithdrawProvider
    {
        public override string Address => "/user/user-withdraw";

        public RemoteUserWithdrawProvider(ActorSystem system, WebSettings configuration) 
            : base(system, configuration.AkkaSettings.AppServerUrl, configuration.AkkaSettings.ActorTimeout)
        {
        }
    }
}