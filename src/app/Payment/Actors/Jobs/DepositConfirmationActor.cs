using Akka.Actor;
using Payment.Contracts.Commands.Waves;
using Payment.Contracts.DataTransfer;
using Payment.Contracts.Events.Forwards;
using Payment.Contracts.Events.Waves;
using Payment.Contracts.Events.Withdraws;
using Payment.Contracts.Providers;
using Persistance.Model.Payments;
using Persistance.Repositories;
using Shared.Configuration;

namespace Payment.Actors.Jobs
{
    public class DepositConfirmationActor : TypedActor,
        IHandle<DepositPlaced>,
        IHandle<TransactionInfo>
    {
        public IWavesActorProvider WavesActorProvider { get; set; }
        public ITransactionManagerActorProvider TransactionActorProvider { get; set; }
        private TransactionActorHelper _helper;

        public AppServerSettings Settings { get; set; }
        public IDepositRepository DepositRepository { get; set; }
        public IAccountRepository AccountRepository { get; set; }
 
        public void Handle(DepositPlaced message)
        {
            WavesActorProvider.Provide().Tell(new GetTransactionConfirmations(message.Deposit.Network, message.Deposit, Self, message.Deposit.TransactionId));
        }

        public void Handle(TransactionInfo message)
        {
            var deposit = (DepositDto)message.Payload;
            if (message.Confirmations == -1)
            {
                DepositRepository.Fail(deposit.Id);
                DepositRepository.SaveChanges();

                Context.System.EventStream.Publish(new DepositFailed(deposit.Id, deposit.Network, deposit.UserName));
            }

            else if (message.Confirmations > Settings.Waves.Transaction.Confirmations)
            {
                DepositRepository.Confirm(deposit.Id);
                DepositRepository.SaveChanges();

                _helper = new TransactionActorHelper(TransactionActorProvider);
                _helper.Deposit(deposit.Network, deposit.UserName, deposit.Amount, deposit.Id);

                if (deposit.IsGameAccount)
                {
                    AccountRepository.Treshold(deposit.Network,
                        deposit.UserName,
                        deposit.Amount,
                        Settings.Waves.Transaction.Fee);

                    AccountRepository.SaveChanges();
                }

                Context.System.EventStream.Publish(new DepositConfirmed(deposit.Id, deposit.Network, deposit.UserName));
            }
        }
    }
}
