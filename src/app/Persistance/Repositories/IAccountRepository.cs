using Persistance.Model.Accounts;
using Shared.Model;

namespace Persistance.Repositories
{
    public interface IAccountRepository
    {
        Account Get(Network network, string userName);
        void Treshold(Network depositNetwork, string depositUserName, long depositAmount, long fee);
        void SaveChanges();
    }
}
