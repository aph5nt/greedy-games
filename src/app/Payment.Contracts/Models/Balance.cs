using Shared.Model;

namespace Payment.Contracts.Models
{
    public class Balance
    {
        public Network Network { get; set; }
        public string UserName { get; set; }
        public long Amount { get; set; }
        public LogEventType? LogEventType { get; set; }
    }
}