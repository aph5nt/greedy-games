using Game.Minefield.Contracts.Model;
using Shared.Model;

namespace Game.Minefield.Contracts.Commands
{
    public class Move : IMinefieldCommand
    {
        public Network Network { get; }
        public string UserName { get;  }
        public string GameId { get;  }
        public Position Position { get; }

        public Move(Network network, string userName, string gameId, Position position)
        {
            Network = network;
            UserName = userName;
            GameId = gameId;
            Position = position;
        }
    }
}