using Payment.Messages.DataTransfer;

namespace Payment.Messages.Events.Withdraws
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