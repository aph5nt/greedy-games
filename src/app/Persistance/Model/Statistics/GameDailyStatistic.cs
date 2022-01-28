using System;
using Shared.Model;

namespace Persistance.Model.Statistics
{
    public class GameDailyStatistic
    {
        public GameTypes Type { get; set; }
        public Network Network { get; set; }
        public string UserName { get; set; }
        public long Profit { get; set; }
        public DateTime Date { get; set; }
    }
}