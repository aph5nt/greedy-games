using Akka.Actor;
using AutoMapper;
using Payment.Contracts.Commands.Waves;
using Payment.Contracts.DataTransfer;
using Payment.Contracts.Events.Dividends;
using Payment.Contracts.Events.Waves;
using Payment.Contracts.Events.Withdraws;
using Payment.Contracts.Providers;
using Persistance.Model.Payments;
using Persistance.Repositories;
using Shared.Configuration;
using Shared.Model;

namespace Payment.Actors.Jobs
{
    public class DividendConfirmationActor : UntypedActor
    {
        public IWavesActorProvider WavesActorProvider { get; set; }
        public ITransactionManagerActorProvider TransactionActorProvider { get; set; }

        public IWithdrawRepository WithdrawRepository { get; set; }
        public AppServerSettings Settings { get; set; }
        public IMapper Mapper { get; set; }
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case DividendWithdrawed command:
                    WavesActorProvider.Provide().Tell(new GetTransactionConfirmations(command.Withdraw.Network, command.Withdraw, Self, command.Withdraw.TransactionSignature));
                    break;
                case TransactionInfo command:

                    var withdraw = (DividendWithdrawDto)command.Payload;
                    var userName = GameTypes.Minefield;
                    if (command.Confirmations == -1)
                    {
                        WithdrawRepository.Fail(withdraw.Id);
                        WithdrawRepository.SaveChanges();

                        var helper = new TransactionActorHelper(TransactionActorProvider);
                        helper.ReleaseWithdrawLock(withdraw.Network, userName.ToString(), withdraw.Amount, withdraw.Id);
                        Context.System.EventStream.Publish(new DividendFailed(Mapper.Map<DividendWithdrawDto>(withdraw)));
                    }

                    else if (command.Confirmations > Settings.Waves.Transaction.Confirmations)
                    {
                        WithdrawRepository.Confirm(withdraw.Id);
                        WithdrawRepository.SaveChanges();

                        var helper = new TransactionActorHelper(TransactionActorProvider);
                        switch (withdraw.WithdrawType)
                        {
                            case WithdrawType.Dividend:
                                helper.Dividend(withdraw.Network, userName, withdraw.Amount, withdraw.Id);
                                break;
                            case WithdrawType.Profit:
                                helper.Profit(withdraw.Network, userName, withdraw.Amount, withdraw.Id);
                                break;
                        }

                        Context.System.EventStream.Publish(new DividendConfirmed(Mapper.Map<DividendWithdrawDto>(withdraw)));
                    }
                    break;
            }
        }
    }
}