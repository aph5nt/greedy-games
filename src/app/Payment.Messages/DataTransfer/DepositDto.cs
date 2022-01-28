using System;
using Shared.Model;

namespace Payment.Messages.DataTransfer
{
    public class DepositDto
    {
        public long Id { get; set; }
        public Network Network { get; set; }
        public string UserName { get; set; }
        public bool IsGameAccount { get; set; }
        public long Amount { get; set; }
        public TranStatus Status { get; set; }
        public string TransactionSignature { get; set; }
        public long TransactionHeight { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}