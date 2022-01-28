using System;
using Akka.Actor;

namespace Shared.Providers
{
    public abstract class RemoteActorProvider : IRemoteActorProvider
    {
        private readonly string _appServerUrl;
        private readonly int _actorTimeout;
        
        protected readonly ActorSystem System;
        protected IActorRef Actor;

        public abstract string Address { get; }

        protected RemoteActorProvider(ActorSystem system, string appServerUrl, int actorTimeout)
        {
            System = system;
            _appServerUrl = appServerUrl;
            _actorTimeout = actorTimeout;
        }

        public virtual  IActorRef Provide()
        {
            if (Actor == null)
            {
                lock (Address)
                {
                    Actor = System.ActorSelection($"{_appServerUrl}{Address}").ResolveOne(TimeSpan.FromSeconds(_actorTimeout)).Result;
                }
            }

            return Actor;
        }
    }
}