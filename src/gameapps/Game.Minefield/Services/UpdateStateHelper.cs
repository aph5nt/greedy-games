using System.Collections.Generic;
using System.Linq;
using Game.Minefield.Contracts.Commands;
using Game.Minefield.Contracts.Model;

namespace Game.Minefield.Services
{
    public static class UpdateStateHelper
    {
        public static void UpdateUserBoard(Move command, State state)
        {
            var result = new List<Field>();

            for (var x = 0; x < state.GameState.Board.GetLength(1); x++)
            for (var y = 0; y < state.GameState.Board.GetLength(0); y++)
            {
                var field = new Field
                {
                    ColumnIndex = x,
                    RowIndex = y,
                    State = FieldState.Unknown,
                    CanStepOn = state.UserState.Status == Status.Alive && x == command.Position.X + 1,
                    SteppedOn = state.SteppedOn(x, y)
                };

                if (x <= command.Position.X)
                    field.State = state.GameState.Board[y, x];

                result.Add(field);
            }

            state.UserState.Board = result.ToArray();
        }

        public static void RevealUserBoard(State state)
        {
            var result = new List<Field>();

            for (var x = 0; x < state.GameState.Board.GetLength(1); x++)
            for (var y = 0; y < state.GameState.Board.GetLength(0); y++)
                result.Add(new Field
                {
                    ColumnIndex = x,
                    RowIndex = y,
                    State = state.GameState.Board[y, x],
                    SteppedOn = state.SteppedOn(x, y)
                });

            state.UserState.Board = result.ToArray();
        }

        private static bool SteppedOn(this State state, int x, int y)
        {
            var result = state.UserState.Moves.Any(q => q.X == x && q.Y == y);
            return result;
        }
    }
}