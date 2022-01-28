using Akka.Actor;
using Payment.Contracts.Commands.Balaces;
using Payment.Contracts.Commands.Transactions;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Payment.Actors
{
    public class TransactionManagerActor : UntypedActor
    {
        private readonly Dictionary<Network, IActorRef> _transactions = new Dictionary<Network, IActorRef>();
        private readonly Dictionary<Network, ActorSelection> _balances = new Dictionary<Network, ActorSelection>();

        private readonly string _connectionString;
        
        public TransactionManagerActor(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case TransactionLogMessage command:
                    _transactions[command.Messages[0].Network].Forward(command);
                    break;
                case BalanceCommand command:
                    _balances[command.Network].ResolveOne(TimeSpan.FromSeconds(3)).Result.Forward(command);
                    break;

                case GetBalance command:
                    _balances[command.Network].ResolveOne(TimeSpan.FromSeconds(3)).Result.Forward(command);
                    break;
            }
        }

        protected override void PreStart()
        {
            foreach (Network network in Enum.GetValues(typeof(Network)))
            {
                _transactions[network] = Context.ActorOf(Props.Create<TransactionActor>(_connectionString), network.ToString().ToLowerInvariant());
                _balances[network] = Context.ActorSelection($"{network.ToString().ToLowerInvariant()}/balance");
            }
 
            base.PreStart();
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: -1,
                withinTimeRange: Timeout.InfiniteTimeSpan,
                decider: Decider.From(x =>
                {
                    if (x is ActorInitializationException)
                    {
                        Thread.Sleep(1000 * 3);
                        return Directive.Restart;
                    }
                    else
                    {
                        return Directive.Restart;
                    }
                })
            );
        }

         
    }
}