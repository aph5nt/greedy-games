using GreedyGames.Services.Dispatchers;
using GreedyGames.Shared.Model;
using GreedyGames.Types;

namespace Game.MineField.ProofOfProfitability
{
    public class EmptyBalanceUpdateDispatcher : IBalanceUpdateDispatcher
    {
        public void Execute(string userName, Network network, long amount)
        {
        }
    }
}