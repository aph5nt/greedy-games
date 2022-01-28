using System;

namespace Game.Minefield.Contracts.Model
{
    [Serializable]
    public class State
    {
        public Settings Settings { get; set; }
        public GameState GameState { get; set; }
        public UserState UserState { get; set; }
    }
}