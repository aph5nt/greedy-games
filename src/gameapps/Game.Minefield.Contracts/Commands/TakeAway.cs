using Shared.Contracts;
using Shared.Model;

namespace Game.Minefield.Contracts.Commands
{
    public class TakeAway : IMinefieldCommand
    {
        public TakeAway(Network network, string userName, string gameId)
        {
            Network = network;
            UserName = userName;
            GameId = gameId;
        }
        
        public Network Network { get;  }
        public string UserName { get; }
        public string GameId { get; }
    }
}