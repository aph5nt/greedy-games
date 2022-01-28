using Payment.Contracts.DataTransfer;

namespace Payment.Contracts.Events.Withdraws
{
    public class DividendFailed
    {
        public DividendWithdrawDto Withdraw { get; set; }

        public DividendFailed(DividendWithdrawDto withdraw)
        {
            Withdraw = withdraw;
        }
    }
}