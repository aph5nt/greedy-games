using Akka.Actor;
using Web.Configuration;

namespace Web.Providers
{
    public class UserWithdrawProvider : ActorProvider, IUserWithdrawProvider
    {
        public override string Address => "/user/user-withdraw";

        public UserWithdrawProvider(ActorSystem system, WebSettings configuration) : base(system, configuration)
        {
        }
    }
}