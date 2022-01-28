using Game.Minefield.Contracts.Model;
using Shared.Contracts;
using Shared.Model;

namespace Game.Minefield.Contracts.Commands
{
    public class TakeWin : IMinefieldCommand
    {
        public TakeWin(Network network, string userName, State state)
        {
            Network = network;
            UserName = userName;
            State = state;
        }
        
        public State State { get;  }
        public Network Network { get; }
        public string UserName { get; }
    }
}