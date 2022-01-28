using Shared.Model;

namespace Game.Minefield.Contracts.Commands
{
    public class Setup : IMinefieldCommand
    {
        public Setup(Network network, string userName, long balanceAmount)
        {
            Network = network;
            UserName = userName;
            BalanceAmount = balanceAmount;
        }
        
        public Network Network { get; }
        public string UserName { get; }
        public long BalanceAmount { get; }
    }
}