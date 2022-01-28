using Payment.Contracts.DataTransfer;

namespace Payment.Contracts.Events.Forwards
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
