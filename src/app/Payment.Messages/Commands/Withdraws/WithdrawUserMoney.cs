using Shared.Model;

namespace Payment.Messages.Commands.Withdraws
{
    public class WithdrawUserMoney
    {
        public WithdrawUserMoney(Network network, string userName, long amount, long fee, string sourceAddress, string targetAddress)
        {
            Network = network;
            UserName = userName;
            Amount = amount;
            Fee = fee;
            SourceAddress = sourceAddress;
            TargetAddress = targetAddress;
        }

        public Network Network { get; set; }
        public string UserName { get; set; }
        public long Amount { get; set; }
        public long Fee { get; set; }
        public string SourceAddress { get; set; }
        public string TargetAddress { get; set; }
    }
}