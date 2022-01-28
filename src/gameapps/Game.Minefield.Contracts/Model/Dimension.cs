using System;

namespace Game.Minefield.Contracts.Model
{
    [Serializable]
    public class Dimension
    {
        public int X { get; set; }
        public int Y { get; set; }

        public static Dimension Default =>
            new Dimension
            {
                X = 6,
                Y = 3
            };
    }
}