using System.Threading.Tasks;
using Persistance.Model.Accounts;
using Shared.Model;

namespace Web.Services
{
    public interface IAccountService
    {
        Task<(string userName, string password)> CreateAsync();
        Task ActivateAsync(Network network, string userName);
        Task<UserAccount> GetAsync(Network network, string userName);
    }
}