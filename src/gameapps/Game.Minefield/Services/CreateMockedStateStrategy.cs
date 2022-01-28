using System.Collections.Generic;
using Game.Minefield.Contracts.Helpers;
using Game.Minefield.Contracts.Model;

namespace Game.Minefield.Services
{
    public class CreateMockedStateStrategy : ICreateStateStrategy
    {
        public State CreateState(Settings settings)
        {
            var board = BoardHelper.FillBoard(settings.Dimension, FieldState.Safe);
            var userBoard = BoardHelper.FillBoard(settings.Dimension, FieldState.Unknown);

            for (var x = 0; x <= board.GetLength(0); x++)
                board[0, x] = FieldState.Mined;

            var position = new Position {X = -1, Y = 0};

            return new State
            {
                Settings = settings,
                GameState = new GameState {Board = board, Size = BoardHelper.GetSize(board)},
                UserState = new UserState
                {
                    Board = userBoard.ToUserBoard(position),
                    FieldSize = GameHelper.GetFieldSize(settings.Dimension),
                    Network = settings.Network,
                    Position = position,
                    Status = Status.Alive,
                    Bet = settings.Bet,
                    Win = 0,
                    Loss = 0,
                    GameId = settings.Id,
                    Moves = new List<Position>()
                }
            };
        }
    }
}