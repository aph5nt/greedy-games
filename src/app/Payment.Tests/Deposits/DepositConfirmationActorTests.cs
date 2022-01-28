using Akka.Actor;
using AutoMapper;
using Moq;
using NUnit.Framework;
using Payment.Contracts.Commands.Balaces;
using Payment.Contracts.DataTransfer;
using Payment.Contracts.Events.Forwards;
using Payment.Contracts.Models;
using Payment.Services;
using Persistance.Model.Payments;
using Shared.Model;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppServer.Tests.Deposits
{
    [TestFixture, Order(2)]
    public class DepositConfirmationActorTests : TestBase
    {
        readonly Mock<IWavesApi> _wavesApiMock = new Mock<IWavesApi>();

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            SetUp.WavesApiFactoryMock
                .Setup(x => x.Create(Network.WAVES))
                .Returns(_wavesApiMock.Object);
        }

        [Test, Order(1)]
        public async Task Should_Confirm_UserAccount_Deposit()
        {
            var txId = "txId";
            var money = 10 * Money.Sathoshi;
            var fee = SetUp.AppServerSettings.Waves.Transaction.Fee;

            _wavesApiMock.
                Setup(x => x.GetTransactionConfirmationstAsync(txId))
                .Returns(Task.FromResult(new GetTransactionConfirmationsResult
                {
                    Confirmations = 200L
                }))
                .Verifiable();
 
            var deposit = SetUp.GetDataContext().Deposits.SingleOrDefault(x => x.TransactionId == txId && x.Status == TranStatus.Pending);
            DepositConfirmationActorRef.Tell(new DepositPlaced(Mapper.Map<DepositDto>(deposit)));

            Thread.Sleep(3000);

            var confirmedDeposit = SetUp.GetDataContext().Deposits.SingleOrDefault(x => x.TransactionId == txId && x.Status == TranStatus.Confirmed);

            Assert.NotNull(confirmedDeposit);

            var balance = await TransactionManagerRef.Ask<Balance>(new GetBalance(deposit.Network, deposit.UserName));

            Assert.True(balance.Amount == money - fee);

            _wavesApiMock.Verify();
        }

        [Test, Order(2)]
        public void Should_Fail_UserAccount_Deposit()
        {
            var txId = "failingTxId";

            _wavesApiMock.
                Setup(x => x.GetTransactionConfirmationstAsync(txId))
                .Returns(Task.FromResult(new GetTransactionConfirmationsResult
                {
                    Confirmations = -1L
                }))
                .Verifiable();

            Deposit deposit;
            using (var context = SetUp.GetDataContext())
            {
                deposit = new Deposit
                {
                    Network = Network.WAVES,
                    UserName = "failinguser",
                    Amount = 100000L,
                    Status = TranStatus.Pending,
                  
                    IsGameAccount = false,
                    TransactionId = txId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Deposits.Add(deposit);
                context.SaveChanges();
            }

            var dto = new DepositPlaced(Mapper.Map<DepositDto>(deposit));
            DepositConfirmationActorRef.Tell(dto);

            Thread.Sleep(3000);

            var failedDeposit = SetUp.GetDataContext().Deposits.SingleOrDefault(x => x.TransactionId == txId && x.Status == TranStatus.Failed);

            Assert.NotNull(failedDeposit);

            _wavesApiMock.Verify();
        }

        // should confirm gamedeposit (and update treshold)

        // should fail gamedeposit
    }
}