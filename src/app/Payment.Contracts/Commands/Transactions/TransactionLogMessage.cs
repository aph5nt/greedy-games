using Payment.Contracts.DataTransfer;

namespace Payment.Contracts.Commands.Transactions
{
    public class TransactionLogMessage
    {
        public TransactionLogDto[] Messages { get; set; }
    }
}