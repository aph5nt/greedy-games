using Akka.Actor;
using Payment.Contracts.Commands.Balaces;
using Payment.Contracts.Commands.Dividends;
using Payment.Contracts.Commands.Waves;
using Payment.Contracts.DataTransfer;
using Payment.Contracts.Events.Dividends;
using Payment.Contracts.Events.Waves;
using Payment.Contracts.Models;
using Payment.Contracts.Providers;
using Persistance.Model.Accounts;
using Persistance.Model.Payments;
using Persistance.Repositories;
using Shared.Configuration;
using Shared.Model;
using System;
using AutoMapper;

namespace Payment.Actors.Jobs
{
    public class DividendActor : TypedActor,
        IHandle<TriggerDividend>,
        IHandle<Balance>
    {
        public IWavesActorProvider WavesActorProvider { get; set; }
        public ITransactionManagerActorProvider TransactionActorProvider { get; set; }

        public IWithdrawRepository WithdrawRepository { get; set; }
        public IAccountRepository AccountRepository { get; set; }
        public AppServerSettings Settings { get; set; }

        public IMapper Mapper { get; set; }

        public void Handle(TriggerDividend message)
        {
            TransactionActorProvider.Provide().Forward(new GetBalance(message.Identity.Network, message.Identity.UserName));
        }

        public void Handle(Balance message)
        {
            var gameAccount = (GameAccount)AccountRepository.Get(message.Network, message.UserName);
            if (message.Amount > gameAccount.Treshold)
            {
                var profitRatio = Settings.Payments.GetBy(message.Network).ProfitRatio;
                var fee = Settings.Waves.Transaction.Fee;

                var spendable = message.Amount - gameAccount.Treshold;
                var profit = (long)(spendable * profitRatio);
                var dividend = profit == 0 ? 0 : spendable - profit;

                if (dividend - fee > fee && profit - fee > fee)
                {
                    var bankAddress = Settings.Payments.GetBy(message.Network).BankAddress;

                    WavesActorProvider.Provide().Forward(new Transfer(
                        message.Network,
                        new DividendData(message.Network, gameAccount.UserName, profit, bankAddress, WithdrawType.Profit),
                        Self,
                        profit - fee,
                        fee,
                        bankAddress,
                        Settings.Payments.GetBy(message.Network).ProfitAddress));

                    WavesActorProvider.Provide().Forward(new Transfer(
                        message.Network,
                        new DividendData(message.Network, gameAccount.UserName, dividend, bankAddress, WithdrawType.Dividend),
                        Self,
                        dividend - fee,
                        fee,
                        bankAddress,
                        Settings.Payments.GetBy(message.Network).DividendAddress));
                }
            }
        }

        public void Handle(Transfered message)
        {
            var payload = (DividendData)message.Payload;
            var withdraw = new DividendWithdraw
            {
                Network = payload.Network,
                Amount = payload.Amount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = TranStatus.Pending,
                GameName = payload.UserName,
                ToAddress = payload.TargetAddress,
                //TransactionHeight = message.TransactionHeight,
                //TransactionSignature = message.TransactionSignature
            };

            WithdrawRepository.Add(withdraw);
            WithdrawRepository.SaveChanges();

            var helper = new TransactionActorHelper(TransactionActorProvider);
            helper.PutWithdrawLock(payload.Network, payload.UserName, payload.Amount, withdraw.Id);

            Context.System.EventStream.Publish(new DividendWithdrawed(Mapper.Map<DividendWithdrawDto>(withdraw)));
        }
    }
}