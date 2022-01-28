using Microsoft.EntityFrameworkCore;
using Persistance;
using Persistance.Model.Statistics;
using Shared.Model;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Services.Impl
{
    public class StatisticService : IStatisticService
    {
        private readonly DataContext _dataContext;

        public StatisticService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<PaginatedList<MinefieldStat>> GetMinefieldStatAsync(Network network, string userName, int page, int pageSize)
        {
            var query = _dataContext.MinefieldStats.Where(q => q.Network == network && q.UserName == userName).OrderByDescending(q => q.Id);

            return await PaginatedList<MinefieldStat>.CreateAsync(query.AsNoTracking(), page, pageSize);
        }

        public async Task<PaginatedList<MinefieldStat>> GetMinefieldStatAsync(Network network, int page, int pageSize)
        {
            var query = _dataContext.MinefieldStats.Where(q => q.Network == network).OrderByDescending(q => q.Id); ;

            return await PaginatedList<MinefieldStat>.CreateAsync(query.AsNoTracking(), page, pageSize);
        }
    }
}
