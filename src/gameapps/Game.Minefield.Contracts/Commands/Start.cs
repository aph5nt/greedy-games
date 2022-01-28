using Game.Minefield.Contracts.Model;
using Shared.Contracts;
using Shared.Model;

namespace Game.Minefield.Contracts.Commands
{
    public class Start : IMinefieldCommand
    {
        public Start(Network network, string userName, Settings settings)
        {
            Network = network;
            UserName = userName;
            Settings = settings;
        }
        
        public Settings Settings { get; }
        public Network Network { get; }
        public string UserName { get; }
    }
}