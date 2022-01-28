using System;
using Shared.Model;

namespace Persistance.Model.Payments
{
    public class Deposit : Identity, ITranStatus
    {
        public bool IsGameAccount { get; set; }
        public long Amount { get; set; }
        public TranStatus Status { get; set; }
        //public string TransactionSignature { get; set; }
        //public long TransactionHeight { get; set; }
        public string TransactionId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}