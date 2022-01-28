using Persistance.Model.Payments;

namespace Persistance.Repositories
{
    public interface IDepositRepository
    {
        void Add(Deposit deposit);
        void SaveChanges();
        void Fail(long depositId);
        void Confirm(long depositId);
    }
}