using System.Collections.Generic;
using System.Threading.Tasks;
using Persistance.Model.Payments;
using Shared.Model;

namespace WebApi.Services
{
    public interface IDepositService
    {
        Task<PaginatedList<Deposit>> GetAsync(Network network, string userName, int page, int pageSize);
        Task TriggerDepositAsync(string userName);
    }
}