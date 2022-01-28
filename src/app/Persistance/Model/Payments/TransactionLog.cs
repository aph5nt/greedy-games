using System;
using Shared.Model;

namespace Persistance.Model.Payments
{
    public class TransactionLog : Identity
    {
        public TransactionLog()
        {
        }

        public TransactionLog(Network network, string userName, LogEventType type, long amount, object messageIdSuffix)
        {
            MessageId = $"{network.ToString()}.{userName}.{type.ToString()}.{messageIdSuffix}";
            Network = network;
            UserName = userName;
            LogEventType = type;
            CreatedAt = DateTime.UtcNow;
            Amount = amount;
            Balance = 0L;
        }
        
        public string MessageId { get; set; }
        public long Amount { get; set; }
        public long Balance { get; set; }
        public LogEventType LogEventType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}