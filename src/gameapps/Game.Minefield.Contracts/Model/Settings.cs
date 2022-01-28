using System;
using Shared.Model;

namespace Game.Minefield.Contracts.Model
{
    [Serializable]
    public class Settings
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public Seed Seed { get; set; }
        public long Bet { get; set; }
        public Dimension Dimension { get; set; }
        public GameTypes GameType { get; set; }
        public Network Network { get; set; }
    }
}