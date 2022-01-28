using System.Threading.Tasks;
using Game.Minefield.Contracts.Model;
using Shared.Model;

namespace Game.Minefield.Storage
{
    public interface IGameStorage
    {
        Task<State> GetAsync(Network network, string userName, string gameId);
        Task<UserState> GetLastAsync(Network network, string userName);
        Task<State> GetLastStateAsync(Network network, string userName);
        Task UpdateAsync(Network network, string userName, string gameId, State state);
        Task InsertAsync(Network network, string userName, string gameId, State state);
    }
}