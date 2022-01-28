using Shared.Model;

namespace Payment.Contracts.Events.Withdraws
{
    public interface IDepositNotification
    {
        Identity Identity { get; }
    }
}