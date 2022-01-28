using Shared.Model;

namespace Payment.Messages.Commands.Balaces
{
    public class GetBalance
    {
        public Network Network { get; }
        public string UserName { get; }

        public GetBalance(Network network, string userName)
        {
            Network = network;
            UserName = userName;
        }
    }
}
