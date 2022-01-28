using Payment.Contracts.DataTransfer;

namespace Payment.Contracts.Events.Withdraws
{
    public class DividendConfirmed
    {
        public DividendWithdrawDto Withdraw { get; set; }

        public DividendConfirmed(DividendWithdrawDto withdraw)
        {
            Withdraw = withdraw;
        }
 
    }
}