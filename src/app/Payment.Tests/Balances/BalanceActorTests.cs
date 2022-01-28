using System;
using Akka.Actor;
using NUnit.Framework;
using Payment.Contracts.Commands.Balaces;
using Payment.Contracts.Events.Balances;
using Payment.Contracts.Models;
using Shared.Contracts;
using Shared.Model;

namespace AppServer.Tests.Balances
{
    [TestFixture]
    public class BalanceActorTests : TestBase
    {
        public const string UserName = "BalanceTest";
 
        [Test]
        public void Should_Load_Balances_After_Start()
        {
            var userName = $"{UserName}1";
            var balance = BalanceActorRef.Ask<Balance>(new GetBalance(Network.FREE, userName)).Result;

            Assert.True(balance.Network == Network.FREE);
            Assert.True(balance.UserName == userName);
            Assert.True(balance.Amount == 100 * Money.Sathoshi);
        }

        [Test]
        public void Should_Validate_Balance()
        {
            var userName = $"{UserName}1";
            var inbox = Inbox.Create(System);
            BalanceActorRef.Tell(
                new VerifyBalance(
                    Network.FREE,
                    userName,
                    100,
                    1,
                    new Object(),
                    inbox.Receiver));

            var verified = inbox.Receive(TimeSpan.FromSeconds(30)) as BalanceVerified;

            Assert.True(verified.Network == Network.FREE);
            Assert.True(verified.UserName == userName);
            Assert.NotNull(verified.Payload);
        }

        [Test]
        public void Should_NotValidate_Balance()
        {
            var userName = $"{UserName}1";
            var inbox = Inbox.Create(System);
           
            inbox.Send(BalanceActorRef, new VerifyBalance(
                Network.FREE,
                userName,
                100 * Money.Sathoshi,
                1000 * Money.Sathoshi,
                new Object(),
                inbox.Receiver));

            var verified = inbox.Receive(TimeSpan.FromSeconds(30)) as Response;

            Assert.False(verified.IsValid);
        }

        [Test]
        public void Should_Update_Existing_Balance()
        {
            var userName = $"{UserName}1";
            BalanceActorRef.Tell(new Balance {Amount = 555 * Money.Sathoshi, Network = Network.FREE, UserName = userName });
            var balance = BalanceActorRef.Ask<Balance>(new GetBalance(Network.FREE, userName)).Result;

            Assert.True(balance.Network == Network.FREE);
            Assert.True(balance.UserName == userName);
            Assert.True(balance.Amount == 555 * Money.Sathoshi);
        }
 
    }
}
