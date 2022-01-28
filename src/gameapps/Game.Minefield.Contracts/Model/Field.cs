using System;
using System.Collections.Generic;
using Shared.Model;

namespace Game.Minefield.Contracts.Model
{
    [Serializable]
    public enum FieldState
    {
        Unknown = 0,
        Mined = 1,
        Safe = 2
    }

    [Serializable]
    public class Field
    {
        public Field()
        {
            CanStepOn = false;
            SteppedOn = false;
        }

        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public FieldState State { get; set; }
        public bool CanStepOn { get; set; }
        public bool SteppedOn { get; set; }
    }

    [Serializable]
    public class FieldSize
    {
        public string Size { get; set; }
        public string Display { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
        public decimal[] Multipiers { get; set; }

        public Dictionary<Network, long> MaxBet { get; set; }
        public Dictionary<Network, long> MinBet { get; set; }
        public Dictionary<Network, long> DefaultBet { get; set; }
    }
}