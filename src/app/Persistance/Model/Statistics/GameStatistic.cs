using System;
using Shared.Model;

namespace Persistance.Model.Statistics
{
    public class GameStatistic
    {
        public long Id { get; set; }
        public string GameId { get; set; }
        public GameTypes Type { get; set; }
        public Network Network { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public string Size { get; set; }
        public int Turn { get; set; }
        public long Bet { get; set; }
        public long Win { get; set; }
        public long Loss { get; set; }
    }
}