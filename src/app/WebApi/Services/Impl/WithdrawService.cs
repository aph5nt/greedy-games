using Microsoft.EntityFrameworkCore;
using Persistance;
using Persistance.Model.Payments;
using Serilog;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Payment.Contracts.Commands.Transactions;
using Payment.Contracts.Providers;
using WebApi.Providers;

namespace WebApi.Services.Impl
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WithdrawService : IWithdrawService
    {
        private readonly DataContext _dataContext;
        private readonly IRemoteTransactionManagerProvider _transactionManagerProvider;

        public WithdrawService(DataContext dataContext, IRemoteTransactionManagerProvider transactionManagerProvider)
        {
            _dataContext = dataContext;
            _transactionManagerProvider = transactionManagerProvider;
        }
        
        public async Task<PaginatedList<UserWithdraw>> GetAsync(Network network, string userName, int page, int pageSize)
        {
            if (network == Network.FREE)
            {
                throw new ArgumentException("Network not supported.");
            }
            var source = _dataContext.Withdraws.OfType<UserWithdraw>()
                .Where(x => x.UserName == userName && x.Network == network)
                .OrderByDescending(x => x.UpdatedAt).AsQueryable();
                
            return await PaginatedList<UserWithdraw>.CreateAsync(source, page, pageSize);
        }

        public async Task WithdrawAsync(string userName, Network network, string destinationAddress, long amount)
        {
            if (network == Network.FREE)
            {
                throw new ArgumentException("Network not supported.");
            }

            using (var transaction = _dataContext.Database.BeginTransaction())
            {
                try
                {
                    var withdraw = new UserWithdraw
                    {
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        UserName = userName,
                        Network = network,
                        Amount = amount,
                        Status = TranStatus.Pending,
                        ToAddress = destinationAddress,
                    };

                    _dataContext.Withdraws.Add(withdraw);
                    await _dataContext.SaveChangesAsync();

                    var transactionHelper = new TransactionActorHelper(_transactionManagerProvider);
                    transactionHelper.PutWithdrawLock(network, userName, amount, withdraw.Id);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Log.Error("Failed to place a withdraw {@Ex}", ex);
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
    
}
