using System.Threading.Tasks;
using Game.Minefield.Storage;
using Shared.Model;

namespace WebApi.Services
{
    public interface IGameLogService
    {
        Task<GameLog> GetAsync(Network network, string userName, string gameId);
    }
}
