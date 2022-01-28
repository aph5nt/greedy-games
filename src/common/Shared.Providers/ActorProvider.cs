using System;
using Akka.Actor;

namespace Shared.Providers
{
    public abstract class ActorProvider : IActorProvider
    {
        private readonly IActorRefFactory _system;
        static readonly object Locker = new object();
        private IActorRef _actor;
        public abstract string Address { get; }

        protected ActorProvider(IActorRefFactory system)
        {
            _system = system;
        }

        public IActorRef Provide()
        {
            lock (Locker)
            {
                if (_actor == null)
                {
                    _actor = _system.ActorSelection(Address).ResolveOne(TimeSpan.FromSeconds(10)).Result;
                }
            }

            return _actor;
        }
    }
}
