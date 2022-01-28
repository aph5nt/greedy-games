using System.Collections.Generic;
using Akka.Actor;
using Shared.Model;
using Web.Configuration;

namespace Web.Providers
{
    public class BalanceProvider : IBalanceProvider
    {
        private string _address => "/user/transaction/{network}/balance";

        readonly Dictionary<Network, ActorSelection> _balances = new Dictionary<Network, ActorSelection>();

        public BalanceProvider(ActorSystem system, WebSettings configuration)
        {
            foreach (var network in new[] { Network.FREE, Network.WAVES, Network.WAVESTEST})
            {
                var selection = system.ActorSelection(configuration.AkkaCfg.AppServerUrl + _address.Replace("{network}", network.ToString().ToLower()));
                _balances.Add(network, selection);
            }
        }
 
        public ActorSelection Provide(Network network)
        {
            return _balances[network];
        }
    }
}