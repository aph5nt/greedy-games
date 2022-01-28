using Payment.Messages.DataTransfer;

namespace Payment.Messages.Events.Dividends
{
    public class DividendWithdrawed
    {
        public DividendWithdrawDto Withdraw { get; set; }

        public DividendWithdrawed(DividendWithdrawDto withdraw)
        {
            Withdraw = withdraw;
        }
    }
}