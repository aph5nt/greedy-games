using System;
using Persistance.Model.Accounts;
using System.Linq;
using Shared.Model;

namespace Persistance.Repositories.Impl
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DataContext _dataContext;

        public AccountRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public Account Get(Network network, string userName)
        {
            return _dataContext.Accounts.Single(x => x.Network == network && x.UserName == userName);
        }

        public void Treshold(Network network, string gameName, long amount, long fee)
        {
            if (Get(network, gameName) is GameAccount account)
            {
                account.Treshold += amount - fee;
                account.UpdatedAt = DateTime.UtcNow;
            }
        }

        public void SaveChanges()
        {
            _dataContext.SaveChanges();
        }
    }
}
