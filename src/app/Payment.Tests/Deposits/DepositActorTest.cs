using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Payment.Contracts.Commands.Forwards;
using Payment.Services;
using Shared.Model;
using Akka.Actor;

namespace AppServer.Tests.Deposits
{
    [TestFixture, Order(1)]
    public class DepositActorTests : TestBase
    {
        readonly Mock<IWavesApi> _wavesApiMock = new Mock<IWavesApi>();
        private string _bankAddress;
        
        [SetUp]
        public override void Setup()
        {
            base.Setup();

            SetUp.WavesApiFactoryMock
                .Setup(x => x.Create(Network.WAVES))
                .Returns(_wavesApiMock.Object);
            
            _bankAddress = SetUp.AppServerSettings.Payments.GetBy(DepositSetup.GameAccount.Network).BankAddress;
        }

        [Test, Order(1)]
        public void Should_Trigger_UserAccountDeposit()
        {
            var money = 10 * Money.Sathoshi;
            var fee = SetUp.AppServerSettings.Waves.Transaction.Fee;
            var txId = "txId";

            _wavesApiMock.
                Setup(x => x.GetBalanceAsync(DepositSetup.UserAccount.DepositAddress))
                .Returns(Task.FromResult(money))
                .Verifiable();
            
            _wavesApiMock.Setup(x => x.TransferAsync(money - fee, fee, DepositSetup.UserAccount.DepositAddress, _bankAddress))
                .Returns(Task.FromResult(new TransferResult
                {
                    TransactionId = txId
                }));

            DepositActorRef.Tell(new TriggerDeposit(new Identity(DepositSetup.UserAccount.Network, DepositSetup.UserAccount.UserName)));

            Thread.Sleep(3000);

            var deposit =
                SetUp.GetDataContext().Deposits
                    .SingleOrDefault(x => x.UserName == DepositSetup.UserAccount.UserName &&
                                          x.Network == DepositSetup.UserAccount.Network &&
                                          x.TransactionId == txId);

            Assert.NotNull(deposit);
            Assert.False(deposit.IsGameAccount);
            Assert.True(deposit.Status == TranStatus.Pending);
            Assert.True(deposit.Amount == money - fee);

            _wavesApiMock.Verify();
        }

        [Test, Order(2)]
        public void Should_Trigger_GameAccountDeposit()
        {
            var money = 10 * Money.Sathoshi;
            var fee = SetUp.AppServerSettings.Waves.Transaction.Fee;
            var txId = "gametxId";

            _wavesApiMock.
                Setup(x => x.GetBalanceAsync(DepositSetup.GameAccount.DepositAddress))
                .Returns(Task.FromResult(money))
                .Verifiable();

            var assetId = SetUp.AppServerSettings.Payments.GetBy(DepositSetup.GameAccount.Network).AssetId;
            var bankAddress = SetUp.AppServerSettings.Payments.GetBy(DepositSetup.GameAccount.Network).BankAddress;
            
            _wavesApiMock.Setup(x => x.TransferAsync(money - fee, fee, DepositSetup.GameAccount.DepositAddress, bankAddress))
                .Returns(Task.FromResult(new TransferResult
                {
                    TransactionId = txId
                }));

            DepositActorRef.Tell(new TriggerDeposit(new Identity(DepositSetup.GameAccount.Network, DepositSetup.GameAccount.UserName)));

            Thread.Sleep(3000);

            var deposit =
                SetUp.GetDataContext().Deposits
                    .SingleOrDefault(x => x.UserName == DepositSetup.GameAccount.UserName &&
                                          x.Network == DepositSetup.GameAccount.Network &&
                                          x.TransactionId == txId);

            Assert.NotNull(deposit);
            Assert.True(deposit.IsGameAccount);
            Assert.True(deposit.Status == TranStatus.Pending);
            Assert.True(deposit.Amount == money - fee);

            _wavesApiMock.Verify();
        }
    }
}
