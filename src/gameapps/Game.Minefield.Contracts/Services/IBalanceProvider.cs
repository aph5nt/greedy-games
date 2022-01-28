using System.Threading.Tasks;
using Shared.Model;

namespace Game.Minefield.Contracts.Services
{
    public interface IBalanceProvider
    {
        long GetBalance(Network network, string userName);
    }
}
