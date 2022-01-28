using Shared.Model;

namespace Payment.Messages.Events.Withdraws
{
    public interface IDepositNotification
    {
        Identity Identity { get; }
    }
}