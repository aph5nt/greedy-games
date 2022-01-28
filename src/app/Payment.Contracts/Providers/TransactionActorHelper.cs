using Akka.Actor;
using Payment.Contracts.Commands.Transactions;
using Payment.Contracts.DataTransfer;
using Shared.Model;
using Shared.Providers;

namespace Payment.Contracts.Providers
{
    public class TransactionActorHelper
    {
        private readonly IActorProvider _managerActorProvider;
      
        public TransactionActorHelper(IActorProvider  managerActorProvider)
        {
            _managerActorProvider = managerActorProvider;
        }

        public void PutGameLock(Network network, string userName, long amount, string gameId)
        {
            Send(network, userName, -amount, LogEventType.GameLock, gameId);
        }

        public void ReleaseGameLock(Network network, string userName, long amount, string gameId)
        {
            Send(network, userName, amount, LogEventType.ReleaseGameLock, gameId);
        }

        public void OnGameLose(Network network, GameTypes gameName, string userName, long amount, string gameId)
        {
            _managerActorProvider.Provide().Tell(new TransactionLogMessage
            {
                Messages = new[]
                {
                    new TransactionLogDto(network, userName, LogEventType.GameLose, - amount, gameId),
                    new TransactionLogDto(network, gameName.ToString(), LogEventType.GameLose, amount, gameId)
                }
            });
        }

        public void OnGameWin(Network network, GameTypes gameName, string userName, long amount, string gameId)
        {
            _managerActorProvider.Provide().Tell(new TransactionLogMessage
            {
                Messages = new[]
                {
                    new TransactionLogDto(network, userName, LogEventType.GameWin, amount, gameId),
                    new TransactionLogDto(network, gameName.ToString(), LogEventType.GameWin, -amount, gameId)
                }
            });
        }

        public void CreateAccount(Network network, string userName, long amount, long ticks)
        {
            Send(network, userName, amount, LogEventType.CreateAccount, ticks);
        }

        public void Deposit(Network network, string userName, long amount, long depositId)
        {
            Send(network, userName, amount, LogEventType.Deposit, depositId);
        }

        public void PutWithdrawLock(Network network, string userName, long amount, long withdrawId)
        {
            Send(network, userName, -amount, LogEventType.WithdrawLock, withdrawId);
        }

        public void ReleaseWithdrawLock(Network network, string userName, long amount, long withdrawId)
        {
            Send(network, userName, amount, LogEventType.ReleaseWithdrawLock, withdrawId);
        }

        public void Withdraw(Network network, string userName, long amount, long withdrawId)
        {
            Send(network, userName, -amount, LogEventType.Withdraw, withdrawId);
        }

        public void Profit(Network network, GameTypes gameName, long amount, long ticks)
        {
            Send(network, gameName.ToString(), -amount, LogEventType.Profit, ticks);
        }

        public void Dividend(Network network, GameTypes gameName, long amount, long ticks)
        {
            Send(network, gameName.ToString(), -amount, LogEventType.Dividend, ticks);
        }

        private void Send(Network network, string userName, long amount, LogEventType logEventType, object messageIdSuffix)
        {
            _managerActorProvider.Provide().Tell(new TransactionLogMessage
            {
                Messages = new[]
                {
                    new TransactionLogDto(network, userName, logEventType, amount, messageIdSuffix),
                }
            });
        }
    }
}