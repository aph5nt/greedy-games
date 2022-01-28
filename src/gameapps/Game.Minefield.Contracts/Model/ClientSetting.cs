using System;

namespace Game.Minefield.Contracts.Model
{
    public class ClientSetting
    {
        public Guid Seed { get; set; }
        public long Bet { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}