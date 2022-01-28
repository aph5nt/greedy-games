using Payment.Contracts.DataTransfer;

namespace Payment.Contracts.Commands.Withdraws
{
    public class ConfirmUserWithdraw
    {
        public ConfirmUserWithdraw(UserWithdrawDto payload)
        {
            Payload = payload;
        }

        public UserWithdrawDto Payload { get; set; }    
    }
}