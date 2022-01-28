using Shared.Model;

namespace Payment.Messages.Models
{
    public class Balance
    {
        public Network Network { get; set; }
        public string UserName { get; set; }
        public long Amount { get; set; }
    }
}