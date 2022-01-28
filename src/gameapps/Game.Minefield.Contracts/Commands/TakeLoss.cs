using Game.Minefield.Contracts.Model;
using Shared.Contracts;
using Shared.Model;

namespace Game.Minefield.Contracts.Commands
{
    public class TakeLoss : IMinefieldCommand
    {
        public TakeLoss(Network network, string userName, State state)
        {
            Network = network;
            UserName = userName;
            State = state;
        }
        
        public State State { get;  }
        public Network Network { get; }
        public string UserName { get;  }
    }
}