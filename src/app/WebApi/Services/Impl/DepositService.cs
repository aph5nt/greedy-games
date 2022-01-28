using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Persistance;
using Persistance.Model.Accounts;
using Persistance.Model.Payments;
using Shared.Model;
using WebApi.Providers;
using Akka.Actor;
using Payment.Contracts.Commands.Forwards;

namespace WebApi.Services.Impl
{
    public class DepositService : IDepositService
    {
        private readonly DataContext _dataContext;
        private readonly IRemoteEventPublisher _eventPublisher;

        public DepositService(DataContext dataContext, IRemoteEventPublisher eventPublisher)
        {
            _dataContext = dataContext;
            _eventPublisher = eventPublisher;
        }

        public async Task<PaginatedList<Deposit>> GetAsync(Network network, string userName, int page, int pageSize)
        {
            if (network == Network.FREE)
            {
                throw new ArgumentException("Network not supported.");
            }

            var source = _dataContext.Deposits
                .Where(x => x.UserName == userName && x.Network == network)
                .OrderByDescending(x => x.UpdatedAt);
               
            return await PaginatedList<Deposit>.CreateAsync(source, page, pageSize);
        }

        public async Task TriggerDepositAsync(string userName)
        {
            var networks = await _dataContext.Accounts.OfType<UserAccount>()
                .Where(x => x.UserName == userName && x.Network != Network.FREE && x.IsActive)
                .Select(x => x.Network)
                .ToListAsync();

            foreach (var network in networks)
            {
                _eventPublisher.Provide().Tell(new TriggerDeposit(new Identity(network, userName)));
            }
        }
    }
}
