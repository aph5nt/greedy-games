using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Persistance;
using Persistance.Model.Statistics;
using Shared.Model;

namespace WebApi.Services.Impl
{
    public class GameStatisticService : IGameStatisticService
    {
        private readonly DataContext _dataContext;

        public GameStatisticService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<PaginatedList<GameStatistic>> GetAsync(Network network, string userName, int page, int pageSize)
        {
            var query = _dataContext.GameStatistics.Where(q => q.Network == network && q.UserName == userName).OrderByDescending(q => q.Id);
            return await PaginatedList<GameStatistic>.CreateAsync(query.AsNoTracking(), page, pageSize);
        }

        public async Task<PaginatedList<GameStatistic>> GetAsync(Network network, int page, int pageSize)
        {
            var query = _dataContext.GameStatistics.Where(q => q.Network == network).OrderByDescending(q => q.Id);
            return await PaginatedList<GameStatistic>.CreateAsync(query.AsNoTracking(), page, pageSize);
        }

        public Task<GameStatistic> GetAsync(Network network, string userName, string gameId)
        {
            return _dataContext.GameStatistics.SingleOrDefaultAsync(x =>
                x.Network == network && x.GameId == gameId && x.UserName == userName);
        }

        public async Task<PaginatedList<GameDailyStatistic>> GetDailyAsync(Network network, int page, int pageSize)
        {
            var today = DateTime.UtcNow.Date;
            var source = _dataContext.GameStatistics.GroupBy(x => new {x.Type, x.UserName, x.Network, x.CreatedAt.Date})
                .Where(x => x.Key.Network == network && x.Key.Date < today)
                .Select(x => new GameDailyStatistic
                {
                    Type = x.Key.Type,
                    UserName = x.Key.UserName,
                    Network = x.Key.Network,
                    Date = x.Key.Date,
                    Profit = x.Sum(r => CalculateProfit(r.Bet, r.Win, r.Loss))
                })
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.Profit);

            return await PaginatedList<GameDailyStatistic>.CreateAsync(source, page, pageSize);
        }

        public async Task<PaginatedList<GameDailyStatistic>> GetDailyAsync(Network network, string userName, int page, int pageSize)
        {
            var source = _dataContext.GameStatistics.GroupBy(x => new {x.Type, x.UserName, x.Network, x.CreatedAt.Date})
                .Where(x => x.Key.Network == network && x.Key.UserName == userName)
                .Select(x => new GameDailyStatistic
                {
                    Type = x.Key.Type,
                    UserName = x.Key.UserName,
                    Network = x.Key.Network,
                    Date = x.Key.Date,
                    Profit = x.Sum(r => CalculateProfit(r.Bet, r.Win, r.Loss))
                })
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.Profit);

            return await PaginatedList<GameDailyStatistic>.CreateAsync(source, page, pageSize);
        }
        
        private static long CalculateProfit(long bet, long win, long loss)
        {
            if (loss > 0)
            {
                return -loss;
            }
            
            return win - bet;
        }
    }
}
