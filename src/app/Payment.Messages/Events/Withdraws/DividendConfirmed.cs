using Payment.Messages.DataTransfer;

namespace Payment.Messages.Events.Withdraws
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