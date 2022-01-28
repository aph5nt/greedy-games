using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using NUnit.Framework;
using Payment.Services;
using Payment.Services.Impl;
using Shared.Configuration;
using Shared.Model;

namespace AppServer.Tests.Manual
{
    
    [TestFixture]
    public class TokenTests
    {
        private string _source = "3PCrudrYHbA3eaTAnEBrBNJvm8QESQeXumj"; // -- address with large number of GREEDYTEST tokens
        private string _target = "3P2T1SRaB5yKyshtWivfP9WVKNDspnYnAXQ";  // -- transfer destiantion address

        private AppServerSettings AppServerSettings = new AppServerSettings();
        private Dictionary<Network, IWavesApi> _apis = new Dictionary<Network, IWavesApi>();
        
        public TokenTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appserver.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appserver.Test.json", optional: false, reloadOnChange: false);
            
            IConfiguration configuration = builder.Build();
            
            var keyVault = configuration.GetSection("KeyVault");
            builder.AddAzureKeyVault(
                $"https://{keyVault["Vault"]}.vault.azure.net/",
                keyVault["ClientId"],
                keyVault["ClientSecret"], new DefaultKeyVaultSecretManager());

            configuration = builder.Build();
            configuration.Bind(AppServerSettings);
            
            _apis[Network.GREEDYTEST] = new WavesApiFactory()
                {
                    Settings = AppServerSettings
                }
                .Create(Network.GREEDYTEST);
            
            _apis[Network.WAVES] = new WavesApiFactory()
                {
                    Settings = AppServerSettings
                }
                .Create(Network.WAVES);
        }
        
        
        [Test]
        public async Task Should_Get_Balances()
        {
            var api = _apis[Network.GREEDYTEST];
            var sourceBalance = await api.GetBalanceAsync(_source);
            var targetBalance = await api.GetBalanceAsync(_target);
            Assert.Greater(sourceBalance, 10);
        }
        
        [Test, Ignore("Manual tests only")]//, Ignore("GREETYTEST token is not accepted by other nodes. Unable to pay assetFee")]
        public async Task Should_Transfer_Asset()
        {
            var api = _apis[Network.GREEDYTEST];
            var beforeSourceBalance = await api.GetBalanceAsync(_source);
            var beforeTargetBalance = await api.GetBalanceAsync(_target);
            
            // set minimum transfer fee on the node first  "3smetxU3pi27KgyapQHKdoS8LxDFtDV9QR67vnW6ZHDJ" = 1000
            var result = await api.TransferAsync(1, 1000, _source, _target);

            Thread.Sleep(10000);
            
            var transaction = await api.GetTransactionAsync(result.TransactionId);
            Assert.True(transaction.Amount == 1);
          
            
            //var afterSourceBalance = await api.GetBalanceAsync(_source);
            //var afterTargetBalance = await api.GetBalanceAsync(_target);
          
            //Assert.Greater(beforeSourceBalance, afterSourceBalance);
            //Assert.Less(beforeTargetBalance, afterTargetBalance);
        }
        
        [Test, Ignore("Manual tests only")]
        public async Task Should_Transfer_Coin()
        {
            var api = _apis[Network.WAVES];
            var beforeSourceBalance = await api.GetBalanceAsync(_source);
            var beforeTargetBalance = await api.GetBalanceAsync(_target);
            
            var result = await api.TransferAsync(1, 1000000, _source, _target);

            Thread.Sleep(10000);
            
            var transaction = await api.GetTransactionAsync(result.TransactionId);
            Assert.True(transaction.Amount == 1);
          
            
            //var afterSourceBalance = await api.GetBalanceAsync(_source);
            //var afterTargetBalance = await api.GetBalanceAsync(_target);
          
            //Assert.Greater(beforeSourceBalance, afterSourceBalance);
            //Assert.Less(beforeTargetBalance, afterTargetBalance);
        }
    }
}