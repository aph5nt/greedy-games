using System;
using NUnit.Framework;
using Shared.Model;

namespace AppServer.Tests.Transactions
{
    [TestFixture]
    public class TransactionManagerActorTest : TestBase
    {
        [Test]
        public void Should_Spawn_TransactionActors_For_All_Networks()
        {
            foreach (Network network in Enum.GetValues(typeof(Network)))
            {
                var selection = $"{ServerAddress}/user/transaction/{network.ToString().ToLower()}/balance";
                System.ActorSelection(selection).ResolveOne(TimeSpan.FromSeconds(30)).Wait();
            }
        }
    }
}