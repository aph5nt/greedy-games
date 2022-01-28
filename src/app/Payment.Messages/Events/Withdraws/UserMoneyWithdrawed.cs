using Payment.Messages.DataTransfer;

namespace Payment.Messages.Events.Withdraws
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