using System;
using Shared.Model;

namespace Payment.Messages.Events.Balances
{
    public class BalanceVerified
    {
        public Network Network { get; }
        public string UserName { get; }
        public Object Payload { get; }
 
        public BalanceVerified(Network network, string userName, object payload)
        {
            Network = network;
            UserName = userName;
            Payload = payload;
        }
    }
}