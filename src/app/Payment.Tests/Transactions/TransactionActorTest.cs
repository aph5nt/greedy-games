using System;
using System.Threading;
using Akka.Actor;
using NUnit.Framework;
using Payment.Contracts.Commands.Balaces;
using Payment.Contracts.Models;
using Shared.Model;

namespace AppServer.Tests.Transactions
{
    [TestFixture]
    public class TransactionActorTest : TestBase
    {
        [Test]
        public void Should_Process_TransactionLogMessage()
        {
            var getBalance = new GetBalance(Network.FREE, "MyFreeUser");

            TransactionActorHelper.CreateAccount(Network.FREE, getBalance.UserName, 1000, DateTime.UtcNow.Ticks);
            Thread.Sleep(600);
            var balance = TransactionManagerRef.Ask<Balance>(getBalance).Result;
            Assert.True(balance.Amount == 1000);

            TransactionActorHelper.Deposit(Network.FREE, getBalance.UserName, 500, DateTime.UtcNow.Ticks + 1);
            Thread.Sleep(600);
            balance = TransactionManagerRef.Ask<Balance>(getBalance).Result;
            Assert.True(balance.Amount == 1500);

            TransactionActorHelper.PutGameLock(Network.FREE, getBalance.UserName, 400, "game1");
            Thread.Sleep(600);
            balance = TransactionManagerRef.Ask<Balance>(getBalance).Result;
            Assert.True(balance.Amount == 1100);

            TransactionActorHelper.ReleaseGameLock(Network.FREE, getBalance.UserName, 400, "game1");
            Thread.Sleep(600);
            balance = TransactionManagerRef.Ask<Balance>(getBalance).Result;
            Assert.True(balance.Amount == 1500);

            TransactionActorHelper.PutWithdrawLock(Network.FREE, getBalance.UserName, 100, 1010);
            Thread.Sleep(600);
            balance = TransactionManagerRef.Ask<Balance>(getBalance).Result;
            Assert.True(balance.Amount == 1400);

            TransactionActorHelper.ReleaseWithdrawLock(Network.FREE, getBalance.UserName, 100, 1010);
            Thread.Sleep(600);
            balance = TransactionManagerRef.Ask<Balance>(getBalance).Result;
            Assert.True(balance.Amount == 1500);

            TransactionActorHelper.OnGameLose(Network.FREE, GameTypes.ToTheMoon, getBalance.UserName, 200, "game2");
            Thread.Sleep(600);
            balance = TransactionManagerRef.Ask<Balance>(getBalance).Result;
            Assert.True(balance.Amount == 1300);

            TransactionActorHelper.OnGameWin(Network.FREE, GameTypes.ToTheMoon, getBalance.UserName, 1000, "game2");
            Thread.Sleep(600);
            balance = TransactionManagerRef.Ask<Balance>(getBalance).Result;
            Assert.True(balance.Amount == 2300);


            TransactionActorHelper.CreateAccount(Network.FREE, GameTypes.Minefield.ToString(), 10000, DateTime.UtcNow.Ticks);
            TransactionActorHelper.Profit(Network.FREE, GameTypes.Minefield, 1000, DateTime.UtcNow.Ticks);
            TransactionActorHelper.Dividend(Network.FREE, GameTypes.Minefield, 1000, DateTime.UtcNow.Ticks);
            Thread.Sleep(600);
            balance = TransactionManagerRef.Ask<Balance>(new GetBalance(Network.FREE, GameTypes.Minefield.ToString())).Result;
            Assert.True(balance.Amount == 8000);
        }
 
    }
}