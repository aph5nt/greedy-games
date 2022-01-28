using Payment.Messages.DataTransfer;

namespace Payment.Messages.Commands.Withdraws
{
    public class ConfirmDividend
    {
        public ConfirmDividend(DividendWithdrawDto payload)
        {
            Payload = payload;
        }

        public DividendWithdrawDto Payload { get; }
    }
}