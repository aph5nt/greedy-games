using Microsoft.EntityFrameworkCore;
using Payment.Messages.Commands.Forwards;
using Persistance;
using Persistance.Model.Accounts;
using Persistance.Model.Payments;
using Shared.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Providers;
using Akka.Actor;

namespace Web.Services.Impl
{
    public class DepositService : IDepositService
    {
        private readonly DataContext _dataContext;
        private readonly IEventPublisher _eventPublisher;

        public DepositService(DataContext dataContext, IEventPublisher eventPublisher)
        {
            _dataContext = dataContext;
            _eventPublisher = eventPublisher;
        }

        public async Task<List<Deposit>> GetAsync(Network network, string userName)
        {
            var results = await _dataContext.Deposits
                .Where(x => x.UserName == userName && x.Network == network)
                .OrderByDescending(x => x.UpdatedAt)
                .Take(256)
                .ToListAsync();

            return results;
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
