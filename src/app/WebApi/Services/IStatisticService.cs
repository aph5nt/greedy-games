using System.Threading.Tasks;
using Persistance.Model.Statistics;
using Shared.Model;

namespace WebApi.Services
{
    public interface IGameStatisticService
    {
        Task<PaginatedList<GameStatistic>> GetAsync(Network network, string userName, int page, int pageSize);
        Task<PaginatedList<GameStatistic>> GetAsync(Network network, int page, int pageSize);
        Task<GameStatistic> GetAsync(Network network, string userName, string gameId);
        Task<PaginatedList<GameDailyStatistic>> GetDailyAsync(Network network, int page, int pageSize);
        Task<PaginatedList<GameDailyStatistic>> GetDailyAsync(Network network, string userName, int page, int pageSize);
    }
}
