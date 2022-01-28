using System;

namespace Game.Minefield.Contracts.Model
{
    [Serializable]
    public class GameState
    {
        public FieldState[,] Board { get; set; }
        public string Size { get; set; }
    }
}