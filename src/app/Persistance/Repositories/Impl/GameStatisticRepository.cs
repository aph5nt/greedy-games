using System.Collections.Generic;
using System.Linq;
using Persistance.Model.Statistics;

namespace Persistance.Repositories.Impl
{
    public class GameStatisticRepository : IGameStatisticRepository
    {
        private readonly DataContext _context;

        public GameStatisticRepository(DataContext context)
        {
            _context = context;
        }

        public void Add(GameStatistic data)
        {
            _context.GameStatistics.Add(data);
        }
 
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}