using System;
using Shared.Model;

namespace Payment.Contracts.DataTransfer
{
    public class DepositDto
    {
        public long Id { get; set; }
        public Network Network { get; set; }
        public string UserName { get; set; }
        public bool IsGameAccount { get; set; }
        public long Amount { get; set; }
        public TranStatus Status { get; set; }
        public string TransactionId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}