using Game.Minefield.Contracts.Commands;
using Game.Minefield.Contracts.Helpers;
using Game.Minefield.Contracts.Model;
using Shared.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Minefield.Validators
{
    public class StartHandlerValidator : HandlerValidator<Start>
    {
        private readonly long _amount;
        private readonly State _state;

        public StartHandlerValidator(long amount, State state)
        {
            _amount = amount;
            _state = state;
        }
        
        protected override IEnumerable<Func<IEnumerable<string>>> CreateValidationPipeline(Start command)
        {
            var validationPipeline = new List<Func<IEnumerable<string>>>
            {
                () => ValidateUserBet(command),
                ValidatePreviousGameStatus,
                () => ValidateBankBalance(command),
                () => ValidateUserBalance(command),
                () => ValidateUserPosition(command)
            };

            return validationPipeline;
        }

        private IEnumerable<string> ValidatePreviousGameStatus()
        {
            if (_state != null && _state.UserState.Status == Status.Alive)
                yield return $"Previous game has not ended.";
        }

        private IEnumerable<string> ValidateBankBalance(Start command)
        {
            var maxMultiplicator = GameHelper.GenerateMultiplicators(command.Settings.Dimension.X,
                command.Settings.Dimension.Y).Max();

            if (command.Settings.Bet * maxMultiplicator >= _amount)
                yield return $"No enough bank funds for {command.Settings.Network} network.";
        }

        private IEnumerable<string> ValidateUserBalance(Start command)
        {
            var total = _amount - command.Settings.Bet;

            if (total < 0m)
                yield return "No enough funds.";
        }

        private IEnumerable<string> ValidateUserPosition(Start command)
        {
            if (_state != null && _state.UserState.Status == Status.None && _state.UserState.Position.X != -1) // start clean game, lose, start another -> state will be invalid
                yield return "Invalid user position";
        }

        private IEnumerable<string> ValidateUserBet(Start command)
        {
            if (command.Settings.Bet < GameHelper.GetMinBet[command.Settings.Network])
                yield return "Bet can nor be negative";

            if (command.Settings.Bet > GameHelper.GetMaxBet[command.Settings.Network])
                yield return "Bet is to high";
        }
    }
}