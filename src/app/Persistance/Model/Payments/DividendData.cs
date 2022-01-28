using Shared.Model;

namespace Persistance.Model.Payments
{
    public class DividendData : Identity
    {
        public DividendData(Network network, string userName, long amount, string targetAddress, WithdrawType withdrawType) 
            : base(network, userName)
        {
            Amount = amount;
            TargetAddress = targetAddress;
            WithdrawType = withdrawType;
        }

        public long Amount { get; set; }
        public WithdrawType WithdrawType { get; set; }
        public string TargetAddress { get; set; }
    }
}