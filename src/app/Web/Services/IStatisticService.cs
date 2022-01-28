using Persistance.Model.Statistics;
using Shared.Model;
using System.Threading.Tasks;

namespace Web.Services
{
    public interface IStatisticService
    {
        Task<PaginatedList<MinefieldStat>> GetMinefieldStatAsync(Network network, string userName, int page, int pageSize);
        Task<PaginatedList<MinefieldStat>> GetMinefieldStatAsync(Network network, int page, int pageSize);
    }
}
