using System;
using Shared.Model;

namespace Payment.Messages.DataTransfer
{
    public class DividendWithdrawDto
    {
        public long Id { get; set; }
        public Network Network { get; set; }
        public string ToAddress { get; set; }
        public long Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string TransactionSignature { get; set; }
        public long TransactionHeight { get; set; }
        public TranStatus Status { get; set; }
        public WithdrawType WithdrawType { get; set; }
        public string GameName { get; set; }
    }
}
