using Persistance.Model.Payments;

namespace Persistance.Repositories
{
    public interface IWithdrawRepository
    {
        void Add(Withdraw withdraw);
        void SaveChanges();
        void Confirm(long id);
        void Fail(long withdrawId);
    }
}