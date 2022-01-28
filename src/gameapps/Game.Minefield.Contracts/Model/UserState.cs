using System;
using System.Collections.Generic;
using Game.Minefield.Contracts.Helpers;
using Shared.Model;

namespace Game.Minefield.Contracts.Model
{
    [Serializable]
    public class UserState
    {
        public static UserState Null()
        {
            return new UserState
            {
                IsEmpty = true
            };
        }
        //public static UserState Empty(Network network, FieldSize fieldSize)
        //{
        //    if (fieldSize == null) fieldSize = GameHelper.DefaultFieldSize;

        //    var emptyBoard = new List<Field>();
        //    for (var r = 0; r < fieldSize.Rows; r++)
        //    {
        //        for (var c = 0; c < fieldSize.Columns; c++)
        //        {
        //            emptyBoard.Add(new Field
        //            {
        //                ColumnIndex = c,
        //                RowIndex = r,
        //                State = FieldState.Unknown,
        //                CanStepOn = false,
        //                SteppedOn = false
        //            });
        //        }
        //    }

        //    return new UserState
        //    {
        //        Board = emptyBoard.ToArray(),
        //        Moves = new List<Position>(),
        //        Network = network,
        //        Status = Status.None,
        //        GameId = string.Empty,
        //        Bet = 0,
        //        Loss = 0,
        //        Win = 0,
        //        Position = new Position { X = -1, Y = 0},
        //        FieldSize = fieldSize
        //    };

        //}
        public bool IsEmpty { get; set; } = false;
        public Field[] Board { get; set; }
        public List<Position> Moves { get; set; }
        public Position Position { get; set; }
        public Status Status { get; set; }
        public long Bet { get; set; }
        public long Win { get; set; }
        public long Loss { get; set; }
        public string GameId { get; set; }
        public FieldSize FieldSize { get; set; }
        public Network Network { get; set; }
    }
}