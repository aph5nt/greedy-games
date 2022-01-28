using Persistance.Model.Accounts;
using Shared.Model;
using System.Threading.Tasks;

namespace WebApi.Services
{
    public interface IAccountService
    {
        Task<(string UserName, string Password)> CreateAsync();
        Task ActivateAsync(Network network, string userName);
        Task<UserAccount> GetUserAccountAsync(Network network, string userName);
        Task<GameAccount> GetGameAccountAsync(Network network, string userName);
        Task<long> CountAsync();
    }
}