using Akka.Actor;
using Payment.Contracts.Commands.Balaces;
using Payment.Contracts.Commands.Waves;
using Payment.Contracts.Commands.Withdraws;
using Payment.Contracts.DataTransfer;
using Payment.Contracts.Events.Balances;
using Payment.Contracts.Events.Waves;
using Payment.Contracts.Events.Withdraws;
using Payment.Contracts.Providers;
using Persistance.Model.Payments;
using Persistance.Repositories;
using Shared.Contracts;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace Payment.Actors
{
    public class UserWithdrawActor : TypedActor,
        IHandle<WithdrawUserMoney>,
        IHandle<BalanceVerified>,
        IHandle<Transfered>
    {
        public IWithdrawRepository WithdrawRepository { get; set; }
        public IWavesActorProvider WavesGatewayProvider { get; set; }
        public ITransactionManagerActorProvider TransactionActorProvider { get; set; } //invalid name: should be BalanceActorProvider
        public IMapper Mapper { get; set; }

        public void Handle(WithdrawUserMoney message)
        {
            var errors = Validate(message);
            if (errors.Any())
            {
                Context.Sender.Tell(new Response(errors.ToArray()));
                return;
            }

            TransactionActorProvider.Provide().Forward(new VerifyBalance(
                message.Network,
                message.UserName,
                message.Amount,
                message.Fee,
                message,
                Self));
        }

        public void Handle(BalanceVerified message)
        {
            var payload = (WithdrawUserMoney)message.Payload;
            WavesGatewayProvider.Provide().Forward(new Transfer(
                message.Network,
                message.Payload,
                Self,
                payload.Amount,
                payload.Fee,
                payload.SourceAddress,
                payload.TargetAddress));
        }

        public void Handle(Transfered message)
        {
            var payload = (WithdrawUserMoney)message.Payload;
            var withdraw = new UserWithdraw
            {
                Network = payload.Network,
                Amount = payload.Amount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = TranStatus.Pending,
                UserName = payload.UserName,
                ToAddress = payload.TargetAddress,
                TransactionId = message.TransactionId
                //TransactionHeight = message.TransactionHeight,
                //TransactionSignature = message.TransactionSignature
            };

            WithdrawRepository.Add(withdraw);
            WithdrawRepository.SaveChanges();

            var helper = new TransactionActorHelper(TransactionActorProvider);
            helper.PutWithdrawLock(payload.Network, payload.UserName, payload.Amount, withdraw.Id);

            Context.System.EventStream.Publish(new UserMoneyWithdrawed(Mapper.Map<UserWithdrawDto>(withdraw)));
            Context.Sender.Tell(new Response());
        }

        private List<string> Validate(WithdrawUserMoney command)
        {
            List<string> errors = new List<string>();

            if (command.Network == Network.FREE)
            {
                errors.Add($"Withdraw from {nameof(command.Network)} is not supported.");
            }

            return errors;
        }
    }
}