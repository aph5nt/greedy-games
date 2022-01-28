using Akka.Actor;
using Payment.Contracts.Commands.Waves;
using Payment.Contracts.Commands.Withdraws;
using Payment.Contracts.DataTransfer;
using Payment.Contracts.Events.Waves;
using Payment.Contracts.Events.Withdraws;
using Payment.Contracts.Providers;
using Persistance.Model.Payments;
using Persistance.Repositories;
using Shared.Configuration;
using Shared.Model;

namespace Payment.Actors.Jobs
{
    public class UserWithdrawConfirmationActor : UntypedActor
    {
        public IWavesActorProvider WavesActorProvider { get; set; }
        public ITransactionManagerActorProvider TransactionActorProvider { get; set; }

        public IWithdrawRepository WithdrawRepository { get; set; }
        public AppServerSettings Settings { get; set; }

        private TransactionActorHelper _helper;

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case ConfirmUserWithdraw command:
                    WavesActorProvider.Provide().Tell(new GetTransactionConfirmations(command.Payload.Network, command.Payload, Self, command.Payload.TransactionSignature));
                    break;
                case TransactionInfo command:
                    var withdraw = (UserWithdrawDto)command.Payload;

                    if (command.Confirmations == -1)
                    {
                        WithdrawRepository.Fail(withdraw.Id);
                        WithdrawRepository.SaveChanges();

                        _helper = new TransactionActorHelper(TransactionActorProvider);
                        _helper.ReleaseWithdrawLock(withdraw.Network, withdraw.UserName, withdraw.Amount, withdraw.Id);
                        Context.System.EventStream.Publish(new UserWithdrawFailed(new Identity
                        {
                            Id = withdraw.Id,
                            Network = withdraw.Network,
                            UserName = withdraw.UserName
                        }));
                    }

                    else if (command.Confirmations > Settings.Waves.Transaction.Confirmations)
                    {
                        WithdrawRepository.Confirm(withdraw.Id);
                        WithdrawRepository.SaveChanges();

                        _helper.ReleaseWithdrawLock(withdraw.Network, withdraw.UserName, withdraw.Amount, withdraw.Id);
                        _helper.Withdraw(withdraw.Network, withdraw.UserName, withdraw.Amount, withdraw.Id);
                        Context.System.EventStream.Publish(new UserWithdrawConfirmed(new Identity
                        {
                            Id = withdraw.Id,
                            Network = withdraw.Network,
                            UserName = withdraw.UserName
                        }));
                    }
                    break;
            }
        }
    }
}