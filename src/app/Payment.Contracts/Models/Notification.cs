using Shared.Model;

namespace Payment.Contracts.Models
{
    public class Notification
    {
        public NotificationType Type { get; set; }
        public string UserName { get; set; }
        public Network Network { get; set; }
        public object Data { get; set; }
    }
}