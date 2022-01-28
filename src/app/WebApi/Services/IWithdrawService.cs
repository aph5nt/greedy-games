using System.Collections.Generic;
using System.Threading.Tasks;
using Persistance.Model.Payments;
using Shared.Model;

namespace WebApi.Services
{
    public interface IWithdrawService
    {
        Task<PaginatedList<UserWithdraw>> GetAsync(Network network, string userName, int page, int pageSize);
        Task WithdrawAsync(string userName, Network network, string destinationAddress, long amount);
    }
}
