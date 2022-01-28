using Akka.Actor;
using AutoMapper;
using Payment.Contracts.Commands.Forwards;
using Payment.Contracts.Commands.Waves;
using Payment.Contracts.DataTransfer;
using Payment.Contracts.Events.Forwards;
using Payment.Contracts.Events.Waves;
using Payment.Contracts.Providers;
using Persistance.Model.Accounts;
using Persistance.Model.Payments;
using Persistance.Repositories;
using Shared.Configuration;
using Shared.Model;
using System;

namespace Payment.Actors.Jobs
{
    public class DepositActor : TypedActor,
        IHandle<TriggerDeposit>,
        IHandle<EffectiveBalance>,
        IHandle<Transfered>
    {
        public IWavesActorProvider WavesActorProvider { get; set; }
        public IDepositRepository DepositRepository { get; set; }
        public IAccountRepository AccountRepository { get; set; }
        public AppServerSettings Settings { get; set; }
        public IMapper Mapper { get; set; }
 
        public void Handle(TriggerDeposit message)
        {
            var account = AccountRepository.Get(message.Identity.Network, message.Identity.UserName);

            switch (account)
            {
                case UserAccount userAccount:
                    WavesActorProvider.Provide().Tell(new GetEffectiveBalance(message.Identity.Network, userAccount, Self, userAccount.DepositAddress, Settings.Waves.Transaction.Confirmations));
                    break;
                case GameAccount gameAccount:
                    WavesActorProvider.Provide().Tell(new GetEffectiveBalance(message.Identity.Network, gameAccount, Self, gameAccount.DepositAddress, Settings.Waves.Transaction.Confirmations));
                    break;
            }
        }

        public void Handle(EffectiveBalance message)
        {
            var payload = (Account)message.Payload;
            var networkFee = Settings.Waves.Transaction.Fee;
            if (message.Amount > networkFee)
            {
                var toSend = message.Amount - networkFee;
                var bankAddress = Settings.Payments.GetBy(payload.Network).BankAddress;
                WavesActorProvider.Provide().Tell(new Transfer(
                    payload.Network,
                    message.Payload, 
                    Self,
                    toSend,
                    networkFee,
                    payload.DepositAddress,
                    bankAddress));
            }
        }

        public void Handle(Transfered message)
        {
            var account = (Account)message.Payload;
            var deposit = new Deposit
            {
                UserName = account.UserName,
                Network = account.Network,
                Amount = message.Amount,
                TransactionId = message.TransactionId,
                CreatedAt = DateTime.UtcNow,
                Status = TranStatus.Pending,
                UpdatedAt = DateTime.UtcNow,
                IsGameAccount = message.Payload is GameAccount
            };

            DepositRepository.Add(deposit);
            DepositRepository.SaveChanges();

            Context.System.EventStream.Publish(new DepositPlaced(Mapper.Map<DepositDto>(deposit)));
        }
    }
}
