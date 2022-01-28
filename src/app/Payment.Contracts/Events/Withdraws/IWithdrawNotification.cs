using Shared.Model;

namespace Payment.Contracts.Events.Withdraws
{
    public interface IWithdrawNotification
    {
        Identity Identity { get; }
    }
}