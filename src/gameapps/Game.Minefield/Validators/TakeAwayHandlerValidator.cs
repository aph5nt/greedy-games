using System;
using System.Collections.Generic;
using Game.Minefield.Contracts.Commands;
using Game.Minefield.Contracts.Model;
using Shared.Infrastructure;

namespace Game.Minefield.Validators
{
    public class TakeAwayHandlerValidator : HandlerValidator<TakeAway>
    {
        private readonly State _state;

        public TakeAwayHandlerValidator(State state)
        {
            _state = state;
        }
        protected override IEnumerable<Func<IEnumerable<string>>> CreateValidationPipeline(TakeAway command)
        {
            var validationPipeline = new List<Func<IEnumerable<string>>>
            {
                () => ValidateInitState(_state),
                () => ValidateStatus(_state),
                () => ValidateMove(_state)
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
                yield return "Status must be Alive";
        }

        private IEnumerable<string> ValidateMove(State state)
        {
            if (state?.UserState.Position.X < 0)
                yield return "At least one move must be made.";
        }
    }
}