using System;
using Akka.Actor;
using Web.Configuration;

namespace Web.Providers
{
    public abstract class ActorProvider : IActorSelectionProvider
    {
        protected readonly ActorSystem System;
        protected readonly WebSettings Configuration;
        protected IActorRef Actor;

        public abstract string Address { get; }

        protected ActorProvider(ActorSystem system, WebSettings configuration)
        {
            System = system;
            Configuration = configuration;
        }

        public virtual IActorRef Provide()
        {
            if (Actor == null)
            {
                lock (Address)
                {
                    Actor = System.ActorSelection($"{Configuration.AkkaCfg.AppServerUrl}{Address}").ResolveOne(TimeSpan.FromSeconds(30)).Result;
                }
            }

            return Actor;
        }
    }
}
