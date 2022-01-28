using System.Threading.Tasks;
using Game.Minefield.Storage;
using Shared.Model;

namespace Web.Services
{
    public interface IGameLogService
    {
        Task<LogDto> GetAsync(Network network, string userName, string gameId);
    }
}
