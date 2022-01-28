using Shared.Model;

namespace Payment.Messages.Commands.Withdraws
{
    public class WithdrawDividend
    {
        public WithdrawDividend(Network network)
        {
            Network = network;
        }

        public Network Network { get; set; }
    }
}