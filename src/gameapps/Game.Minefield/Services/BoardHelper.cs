using Game.Minefield.Contracts.Model;
using System;
using System.Collections.Generic;
using Game.Minefield.Contracts.Helpers;

namespace Game.Minefield.Services
{
    public static class BoardHelper
    {
        public static string GetSize(FieldState[,] board)
        {
            var xlen = board.GetLength(1);
            var ylen = board.GetLength(0);

            var sum = xlen + ylen;

            switch (sum)
            {
                case 5:
                    return "3 x 2";
                case 9:
                    return "6 x 3";
                case 13:
                    return "9 x 4";
                default:
                    throw new NotImplementedException();
            }
        }

        public static long GetWin(State state)
        {
            var xlen = state.GameState.Board.GetLength(1);
            var ylen = state.GameState.Board.GetLength(0);
            var multiplicators = GameHelper.GenerateMultiplicators(xlen, ylen);
            return (long) (state.UserState.Bet * multiplicators[state.UserState.Position.X]);
        }

        public static FieldState[,] FillBoard(Dimension dimension, FieldState defaultField)
        {
            var board = new FieldState[dimension.Y, dimension.X];
            for (var y = 0; y < board.GetLength(0); y++)
            for (var x = 0; x < board.GetLength(1); x++)
                board[y, x] = defaultField;

            return board;
        }

        public static Field[] ToUserBoard(this FieldState[,] board, Position position)
        {
            var result = new List<Field>();

            for (var y = 0; y < board.GetLength(0); y++)
            for (var x = 0; x < board.GetLength(1); x++)
                result.Add(new Field
                {
                    ColumnIndex = x,
                    RowIndex = y,
                    State = board[y, x],
                    CanStepOn = x == 0
                });
            return result.ToArray();
        }
    }
}