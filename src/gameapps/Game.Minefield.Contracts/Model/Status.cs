using System;

namespace Game.Minefield.Contracts.Model
{
    [Serializable]
    public enum Status
    {
        None = 0,
        Alive = 1,
        Dead = 2,
        Escaped = 3,
        TakeAway = 4
    }
}