using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using NUnit.Framework;
using Payment.Actors.Jobs;
using Payment.Contracts.Commands.Forwards;
using Payment.Contracts.DataTransfer;
using Payment.Contracts.Events.Forwards;
using Payment.Contracts.Events.Withdraws;
using Quartz;
using Shared.Model;

namespace AppServer.Tests.Deposits
{
    [TestFixture, Order(3)]
    public class DepositSchedulerActorTests : TestBase
    {
        private Network _network = Network.WAVES;
        private string _userName = "usertest1";
        private string _group = "deposits";

        [Test, Order(1)]
        public async Task Should_Schedule_On_TriggerDeposit()
        {
            var @event = new TriggerDeposit(new Identity(1, _network, _userName));
            DepositSchedulerActorRef.Tell(@event);
            Thread.Sleep(3000);

            var exist = await AppService.Scheduler.CheckExists(new TriggerKey(DepositSchedulerActor.Name(@event), _group));
            Assert.True(exist);
        }

        [Test, Order(2)]
        public async Task Should_Schedule_On_DepositPlaced()
        {
            var event1 = new DepositPlaced(new DepositDto
            {
                Network = _network,
                UserName = _userName,
                Id = 1L
            });

            var event2 = new DepositPlaced(new DepositDto
            {
                Network = _network,
                UserName = _userName,
                Id = 2L
            });

            DepositSchedulerActorRef.Tell(event1);
            DepositSchedulerActorRef.Tell(event2);

            Thread.Sleep(3000);

            Assert.True(await AppService.Scheduler.CheckExists(new TriggerKey(DepositSchedulerActor.Name(event1), _group)));
            Assert.True(await AppService.Scheduler.CheckExists(new TriggerKey(DepositSchedulerActor.Name(event2), _group)));
        }

        [Test, Order(3)]
        public async Task Should_Schedule_On_DepositConfirmed()
        {
            var @event = new DepositConfirmed(new Identity(1, _network, _userName));
            DepositSchedulerActorRef.Tell(@event);

            Thread.Sleep(1500);

            var exist = await AppService.Scheduler.CheckExists(new TriggerKey(DepositSchedulerActor.Name(@event), _group));
            Assert.False(exist);
        }

        [Test, Order(4)]
        public async Task Should_Schedule_On_DepositFailed()
        {
            var @event = new DepositFailed(new Identity(1, _network, _userName));
            DepositSchedulerActorRef.Tell(@event);

            Thread.Sleep(1500);

            var exist = await AppService.Scheduler.CheckExists(new TriggerKey(DepositSchedulerActor.Name(@event), _group));
            Assert.False(exist);
        }
    }
}