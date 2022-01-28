using Payment.Contracts.DataTransfer;

namespace Payment.Contracts.Commands.Withdraws
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