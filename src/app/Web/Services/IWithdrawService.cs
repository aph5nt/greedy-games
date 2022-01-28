using Persistance.Model.Payments;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Model;

namespace Web.Services
{
    public interface IWithdrawService
    {
        Task<List<UserWithdraw>> GetAsync(Network network, string identityName);
        Task WithdrawAsync(string userName, Network network, string destinationAddress, long amount);
    }
}
