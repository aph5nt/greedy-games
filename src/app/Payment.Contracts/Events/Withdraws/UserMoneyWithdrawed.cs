using Payment.Contracts.DataTransfer;

namespace Payment.Contracts.Events.Withdraws
{
    public class UserMoneyWithdrawed
    {
        public UserMoneyWithdrawed(UserWithdrawDto payload)
        {
            Payload = payload;
        }

        public UserWithdrawDto Payload { get; }
    }
}