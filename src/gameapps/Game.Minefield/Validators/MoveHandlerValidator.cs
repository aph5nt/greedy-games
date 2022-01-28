using System;
using System.Collections.Generic;
using Game.Minefield.Contracts.Commands;
using Game.Minefield.Contracts.Model;
using Shared.Infrastructure;

namespace Game.Minefield.Validators
{
    public class MoveHandlerValidator : HandlerValidator<Move>
    {
        private readonly State _state;

        public MoveHandlerValidator(State state)
        {
            _state = state;
        }
        protected override IEnumerable<Func<IEnumerable<string>>> CreateValidationPipeline(Move command)
        {
            var validationPipeline = new List<Func<IEnumerable<string>>>
            {
                () => ValidateInitState(_state),
                () => ValidateStatus(_state),
                () => ValidateMove(command, _state)
            };

            return validationPipeline;
        }
 
        private IEnumerable<string> ValidateInitState(State state)
        {
            if (state == null)
                yield return "The game has been not initialized";
        }

        private IEnumerable<string> ValidateStatus(State state)
        {
            if (state?.UserState.Status != Status.Alive)
                yield return "Status is not Alive";
        }

        private IEnumerable<string> ValidateMove(Move command, State state)
        {
            var isOnTheBoard = command.Position.X <= state.GameState.Board.GetLength(1) && command.Position.X >= 0 &&
                               command.Position.Y <= state.GameState.Board.GetLength(0) && command.Position.Y >= 0;

            if (!isOnTheBoard)
                yield return "Move is not allowed.";

            var moveForward = command.Position.X > state.UserState.Position.X &&
                              command.Position.X - state.UserState.Position.X == 1;

            if (!moveForward)
                yield return "Move is not allowed.";
        }
    }
}