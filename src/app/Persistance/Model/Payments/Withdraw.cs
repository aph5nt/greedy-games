using System;
using Shared.Model;

namespace Persistance.Model.Payments
{
    public interface ITranStatus
    {
        TranStatus Status { get; set; }
    }

    public abstract class Withdraw : ITranStatus
    {
        public long Id { get; set; }
        public Network Network { get; set; }
        public string ToAddress { get; set; }
        public long Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string TransactionId { get; set; }
        public TranStatus Status { get; set; }
    }

    public class UserWithdraw : Withdraw
    {
        public string UserName { get; set; }
    }

    public class DividendWithdraw : Withdraw
    {
        public WithdrawType WithdrawType { get; set; }
        public string GameName { get; set; }
    }

  
}