using Akka.Actor;
using Akka.Event;
using Game.Minefield.Contracts.Commands;
using Game.Minefield.Contracts.Helpers;
using Game.Minefield.Contracts.Model;
using Game.Minefield.Services;
using Game.Minefield.Storage;
using Game.Minefield.Validators;
using Payment.Contracts.Providers;
using Shared.Contracts;
using Shared.Model;
using System;
using System.Linq;
using Payment.Contracts.Commands.Transactions;
using Payment.Contracts.Models;
using Persistance.Model.Statistics;
using Persistance.Repositories;
using Status = Game.Minefield.Contracts.Model.Status;

namespace Game.Minefield.Actors
{
#pragma warning disable 618
    public class GameActor : TypedActor,
#pragma warning restore 618
        IHandle<Setup>,
        IHandle<GetUserState>,
        IHandle<Start>,
        IHandle<Move>,
        IHandle<TakeAway>,
        IHandle<Terminate>,
        IHandle<ReceiveTimeout>
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();
        private readonly TransactionActorHelper _transactionActorHelper;
        private  Network _network;
        private string _userName;
        private long _amount;
        private State _state;
        
        public IGameStorage GameStorage { get; set; }
        public ILogStorage LogStorage { get; set; }
        public IGameStatisticRepository StatisticRepository { get; set; }
        public ICreateStateStrategy CreateStateStrategy { get; set; }

        public GameActor(ITransactionManagerActorProvider transactionActorProvider)
        {
            _transactionActorHelper = new TransactionActorHelper(transactionActorProvider);
            SetReceiveTimeout(TimeSpan.FromMinutes(10));
        }
 
        public void Handle(GetUserState message)
        {
            if (_state != null)
            {
                Context.Sender.Tell(new Response<UserState>(_state.UserState));
            }

            if (_state == null)
            {
                _state = GameStorage.GetLastStateAsync(message.Network, message.UserName).Result;
                var emptyState = UserState.Null();

                Context.Sender.Tell(_state == null
                    ? new Response<UserState>(emptyState)
                    : new Response<UserState>(_state?.UserState));
            }
        }

        public void Handle(Setup message)
        {
            _log.Debug($"Setting up {nameof(GameActor)}");
            
            _userName = message.UserName;
            _network = message.Network;
            _amount = message.BalanceAmount;
            _state = GameStorage.GetLastStateAsync(_network, _userName).Result;

        }

        public void Handle(Balance message)
        {
            _log.Debug($"Updating balance for {_network}.{_userName}: previous: {_amount}, new {message.Amount}");
            
            _amount = message.Amount;
        }

        public void Handle(Start message)
        {
            var errors = new StartHandlerValidator(_amount, _state).Validate(message);
            if (errors.Any())
            {
                _log.Debug("Start message is invalid");
                Context.Sender.Tell(new Response<UserState>(errors));
            }
            else
            {
                _state = CreateStateStrategy.CreateState(message.Settings);
                _transactionActorHelper.PutGameLock(message.Settings.Network, message.Settings.UserName, message.Settings.Bet, message.Settings.Id);

                GameStorage.InsertAsync(message.Settings.Network, message.Settings.UserName, message.Settings.Id, _state).PipeTo(Context.Self);
                LogStorage.InsertAsync(message.Settings).PipeTo(Context.Self);

                Context.Sender.Tell(new Response<UserState>(_state.UserState));
                
                _log.Debug("Started new game");
            }
        }
 
        public void Handle(Move message)
        {
            var errors = new MoveHandlerValidator(_state).Validate(message);
            if (errors.Any())
            {
                _log.Debug("Move message is invalid");
                Context.Sender.Tell(new Response<UserState>(errors));
            }
            else
            {
                UpdateState(message, _state);
                OnWin(Context);
                OnLoss(Context);
                GameStorage.UpdateAsync(message.Network, message.UserName, message.GameId, _state).PipeTo(Context.Self);
                Context.Sender.Tell(new Response<UserState>(_state.UserState));
                _log.Debug("Moved to another field");
            }
        }
       
        public void Handle(TakeAway message)
        {
            var errors = new TakeAwayHandlerValidator(_state).Validate(message);
            if (errors.Any())
            {
                _log.Debug("TakeAway message is invalid");
                Context.Sender.Tell(new Response<UserState>(errors));
            }
            else
            {
                _state.UserState.Status = Status.TakeAway;
                UpdateStateHelper.RevealUserBoard(_state);

                OnWin(Context);
                GameStorage.UpdateAsync(message.Network, message.UserName, message.GameId, _state).PipeTo(Context.Self);
                Context.Sender.Tell(new Response<UserState>(_state.UserState));
                _log.Debug("Taken away win");
            }
        }

        private void UpdateState(Move command, State state)
        {
            switch (state.GameState.Board[command.Position.Y, command.Position.X])
            {
                //case FieldState.Unknown:
                //    throw new GameException(new[] { "Invalid board state" });
                case FieldState.Mined:
                    state.UserState.Status = Status.Dead;
                    state.UserState.Position.X = command.Position.X;
                    state.UserState.Position.Y = command.Position.Y;
                    state.UserState.Moves.Add(new Position { X = command.Position.X, Y = command.Position.Y });

                    UpdateStateHelper.RevealUserBoard(state);
                    break;
                case FieldState.Safe:
                    if (state.GameState.Board.GetLength(1) - 1 == command.Position.X)
                    {
                        state.UserState.Status = Status.Escaped;
                        state.UserState.Position.X = command.Position.X;
                        state.UserState.Position.Y = command.Position.Y;
                        state.UserState.Moves.Add(new Position { X = command.Position.X, Y = command.Position.Y });
                        UpdateStateHelper.RevealUserBoard(state);
                    }
                    else
                    {
                        state.UserState.Status = Status.Alive;
                        state.UserState.Position.X = command.Position.X;
                        state.UserState.Position.Y = command.Position.Y;
                        state.UserState.Moves.Add(new Position { X = command.Position.X, Y = command.Position.Y });
                        UpdateStateHelper.UpdateUserBoard(command, state);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnWin(IActorContext context)
        {
            if (_state.UserState.Status == Status.Escaped || _state.UserState.Status == Status.TakeAway)
            {
                _state.UserState.Win = BoardHelper.GetWin(_state);
                var amount = _state.UserState.Win - _state.UserState.Bet;

                _transactionActorHelper.OnGameWin(
                    _state.Settings.Network,
                    GameTypes.Minefield,
                    _state.Settings.UserName,
                    amount,
                    _state.Settings.Id);

                _transactionActorHelper.ReleaseGameLock(
                    _state.Settings.Network,
                    _state.Settings.UserName,
                    _state.Settings.Bet,
                    _state.Settings.Id);

                StatisticRepository.Add(new GameStatistic
                {
                    Network = _state.Settings.Network,
                    UserName = _state.Settings.UserName,
                    Type = GameTypes.Minefield,
                    Win = _state.UserState.Win,
                    Bet = _state.Settings.Bet,
                    CreatedAt = DateTime.UtcNow,
                    GameId = _state.Settings.Id,
                    Loss = _state.UserState.Loss,
                    Size = _state.GameState.Size,
                    Turn = _state.UserState.Position.X + 1
                });
                
                StatisticRepository.SaveChanges();

                context.System.EventStream.Publish(_state);
            }
        }

        private void OnLoss(IActorContext context)
        {
            if (_state.UserState.Status == Status.Dead)
            {
                _state.UserState.Loss = _state.UserState.Bet;
                _transactionActorHelper.OnGameLose(
                    _state.Settings.Network,
                    GameTypes.Minefield,
                    _state.Settings.UserName,
                    _state.Settings.Bet,
                    _state.Settings.Id);

                _transactionActorHelper.ReleaseGameLock(
                    _state.Settings.Network,
                    _state.Settings.UserName,
                    _state.Settings.Bet,
                    _state.Settings.Id);

                StatisticRepository.Add(new GameStatistic
                {
                    Network = _state.Settings.Network,
                    UserName = _state.Settings.UserName,
                    Type = GameTypes.Minefield,
                    Win = _state.UserState.Win,
                    Bet = _state.Settings.Bet,
                    CreatedAt = DateTime.UtcNow,
                    GameId = _state.Settings.Id,
                    Loss = _state.UserState.Loss,
                    Size = _state.GameState.Size,
                    Turn = _state.UserState.Position.X + 1
                });
                
                StatisticRepository.SaveChanges();

                context.System.EventStream.Publish(_state);
            }
        }

        public void Handle(Terminate message)
        {
            Context.Self.GracefulStop(TimeSpan.FromSeconds(30)).PipeTo(Context.Sender);
        }

        public void Handle(ReceiveTimeout message)
        {
            Context.Self.Tell(PoisonPill.Instance);
        }
    }
}
