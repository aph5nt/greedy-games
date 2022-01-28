using System.Threading.Tasks;
using Game.Minefield.Contracts.Model;
using Shared.Model;

namespace Game.Minefield.Storage
{
    public interface ILogStorage
    {
        Task InsertAsync(Settings settings);
        Task<LogEntity> GetAsync(Network network, string userName, string gameId);
    }
}