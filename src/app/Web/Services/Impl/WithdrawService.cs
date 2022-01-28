using Microsoft.EntityFrameworkCore;
using Payment.Messages.Commands.Transactions;
using Persistance;
using Persistance.Model.Payments;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Providers;

namespace Web.Services.Impl
{
    public class WithdrawService : IWithdrawService
    {
        private DataContext _dataContext;
        private ITransactionManagerProvider _transactionManagerProvider;

        public WithdrawService(DataContext dataContext, ITransactionManagerProvider transactionManagerProvider)
        {
            _dataContext = dataContext;
            _transactionManagerProvider = transactionManagerProvider;
        }
        public async Task<List<UserWithdraw>> GetAsync(Network network, string userName)
        {
            var results = await _dataContext.Withdraws.OfType<UserWithdraw>()
                .Where(x => x.UserName == userName && x.Network == network)
                .OrderByDescending(x => x.UpdatedAt)
                .Take(256)
                .ToListAsync();

            return results;
        }

        public async Task WithdrawAsync(string userName, Network network, string destinationAddress, long amount)
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

            var transactionHelper = new TransactionActorSelectionHelper(_transactionManagerProvider.Provide());
            transactionHelper.PutWithdrawLock(network, userName, amount, withdraw.Id);
        }
    }
    
}
