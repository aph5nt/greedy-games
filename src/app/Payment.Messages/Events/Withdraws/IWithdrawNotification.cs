using Shared.Model;

namespace Payment.Messages.Events.Withdraws
{
    public interface IWithdrawNotification
    {
        Identity Identity { get; }
    }
}