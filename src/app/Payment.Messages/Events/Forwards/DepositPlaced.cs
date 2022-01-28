using Payment.Messages.DataTransfer;

namespace Payment.Messages.Events.Forwards
{
    public class DepositPlaced
    {
        public DepositDto Deposit { get; set; }
        public DepositPlaced(DepositDto deposit)
        {
            Deposit = deposit;
        }
 
    }
}
