using Akka.Actor;
using Game.Minefield.Contracts.Services;
using Shared.Model;
using System;
using Payment.Contracts.Commands.Balaces;
using Payment.Contracts.Models;

namespace AppServer.Providers
{
    public class BalanceProvider : IBalanceProvider
    {
        private readonly ActorSystem _system;
        private IActorRef _actorRef;
        private readonly object _locker = new object();
     
        public BalanceProvider(ActorSystem system)
        {
            _system = system;
        }

        public long GetBalance(Network network, string userName)
        {
            lock (_locker)
            {
                if (_actorRef == null)
                {
                    _actorRef = _system.ActorSelection($"user/transaction/{network.ToString().ToLowerInvariant()}/balance")
                        .ResolveOne(TimeSpan.FromSeconds(30)).Result;
                }
            }

            return _actorRef
                .Ask<Balance>(new GetBalance(network, userName))
                .Result
                .Amount;
        }
    }
}