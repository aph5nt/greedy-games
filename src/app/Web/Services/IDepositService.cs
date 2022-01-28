using System.Collections.Generic;
using Persistance.Model.Payments;
using System.Threading.Tasks;
using Shared.Model;

namespace Web.Services
{
    public interface IDepositService
    {
        Task<List<Deposit>> GetAsync(Network network, string userName);
        Task TriggerDepositAsync(string userName);
    }
}