using Shared.Model;

namespace Game.Minefield.Contracts.Commands
{
    public class GetUserState : IMinefieldCommand
    {
        public GetUserState(Network network, string userName)
        {
            Network = network;
            UserName = userName;
        }

        public Network Network { get; }
        public string UserName { get; }
    }
}