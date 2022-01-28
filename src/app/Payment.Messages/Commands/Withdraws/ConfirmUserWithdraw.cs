using Payment.Messages.DataTransfer;

namespace Payment.Messages.Commands.Withdraws
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