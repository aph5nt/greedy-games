using Akka.Actor;
using Payment.Messages.DataTransfer;
using Shared.Model;

namespace Payment.Messages.Commands.Transactions
{
    public class TransactionActorHelper
    {
        private IActorRef TransactionActorRef { get; set; }
      
        public TransactionActorHelper(ITransactionManagerActorProvider transactionActorRef)
        {
            TransactionActorRef = transactionActorRef;
        }

        public void PutGameLock(Network network, string userName, long amount, string gameId)
        {
            Send(network, userName, -amount, LogEventType.GameLock, gameId);
        }

        public void ReleaseGameLock(Network network, string userName, long amount, string gameId)
        {
            Send(network, userName, amount, LogEventType.ReleaseGameLock, gameId);
        }

        public void OnGameLose(Network network, string userName, long amount, string gameId)
        {
            TransactionActorRef.Tell(new TransactionLogMessage
            {
                Messages = new[]
                {
                    new TransactionLogDto(network, userName, LogEventType.GameLose, - amount, gameId),
                    new TransactionLogDto(network, GameTypes.Minefield.ToString(), LogEventType.GameLose, amount, gameId)
                }
            });
        }

        public void OnGameWin(Network network, string userName, long amount, string gameId)
        {
            TransactionActorRef.Tell(new TransactionLogMessage
            {
                Messages = new[]
                {
                    new TransactionLogDto(network, userName, LogEventType.GameWin, amount, gameId),
                    new TransactionLogDto(network, GameTypes.Minefield.ToString(), LogEventType.GameWin, -amount, gameId)
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

        public void Profit(Network network, string userName, long amount, long ticks)
        {
            Send(network, userName, -amount, LogEventType.Profit, ticks);
        }

        public void Dividend(Network network, string userName, long amount, long ticks)
        {
            Send(network, userName, -amount, LogEventType.Dividend, ticks);
        }

        private void Send(Network network, string userName, long amount, LogEventType logEventType, object messageIdSuffix)
        {
            TransactionActorRef.Tell(new TransactionLogMessage
            {
                Messages = new[]
                {
                    new TransactionLogDto(network, userName, logEventType, amount, messageIdSuffix),
                }
            });
        }
    }
}