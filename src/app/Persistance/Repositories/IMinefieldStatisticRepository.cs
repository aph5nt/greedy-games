using System.Collections.Generic;
using Persistance.Model.Statistics;

namespace Persistance.Repositories
{
    public interface IGameStatisticRepository
    {
        void Add(GameStatistic statistic);
        void SaveChanges();
    }
}