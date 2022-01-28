using Payment.Messages.DataTransfer;

namespace Payment.Messages.Commands.Transactions
{
    public class TransactionLogMessage
    {
        public TransactionLogDto[] Messages { get; set; }
    }
}