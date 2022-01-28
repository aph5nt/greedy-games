using Payment.Contracts.DataTransfer;

namespace Payment.Contracts.Events.Dividends
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