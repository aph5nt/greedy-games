using System;
using Shared.Model;

namespace Payment.Contracts.DataTransfer
{
    public class TransactionLogDto
    {
        public TransactionLogDto()
        {
            
        }

        public TransactionLogDto(Network network, string userName, LogEventType type, long amount, object messageIdSuffix)
        {
            MessageId = $"{network.ToString()}.{userName}.{type.ToString()}.{messageIdSuffix}";
            Network = network;
            UserName = userName;
            LogEventType = type;
            CreatedAt = DateTime.UtcNow;
            Amount = amount;
            Balance = 0L;
        }
 
        public long Id { get; set; }
        public Network Network { get; set; }
        public string UserName { get; set; }
        public string MessageId { get; set; }
        public long Amount { get; set; }
        public long Balance { get; set; }
        public LogEventType LogEventType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
