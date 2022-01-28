using Akka.Actor;
using Akka.DI.Core;
using Game.Minefield.Contracts.Commands;
using Shared.Model;
using System;
using System.Collections.Generic;
using Game.Minefield.Contracts.Services;
using Game.Minefield.Services;
using Payment.Contracts.Models;

namespace Game.Minefield.Actors
{
#pragma warning disable 618
    public class GameManagerActor : TypedActor,
#pragma warning restore 618
        IHandle<IMinefieldCommand>,
        IHandle<Balance>,
        IHandle<Terminated>
    {
        public Dictionary<IActorRef, string> GamesByRef = new Dictionary<IActorRef, string>();
        public Dictionary<string, IActorRef> Games = new Dictionary<string, IActorRef>();
        public IBalanceProvider BalanceProvider { get; set; }

        protected override void PreStart()
        {
            Context.System.EventStream.Subscribe(Context.Self, typeof(Balance));
        }

        public void Handle(Balance message)
        {
            var key = Key(message.Network, message.UserName);
            if (Games.TryGetValue(key, out var gameActor))
            {
                gameActor.Forward(message);
            }
        }
        
        public void Handle(IMinefieldCommand message)
        {
            var key = Key(message.Network, message.UserName);
            if (!Games.ContainsKey(key))
            {
                Games[key] = Context.ActorOf(Context.DI().Props<GameActor>().WithMailbox("balance-priority-mailbox"), key);
                Context.Watch(Games[key]);
                GamesByRef.TryAdd(Games[key], key);
                
                Games[key].Forward(new Setup(message.Network, message.UserName, BalanceProvider.GetBalance(message.Network, message.UserName)));
            }

            Games[key].Forward(message);
        }
 
        public void Handle(Terminated message)
        {
            if (GamesByRef.TryGetValue(message.ActorRef, out var key))
            {
                Context.Unwatch(message.ActorRef);
                GamesByRef.Remove(message.ActorRef);
                Games.Remove(key);
            }
        }
        
        private string Key(Network network, string userName)
        {
            return $"{network}.{userName}";
        }
    }
}