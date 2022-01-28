using System;
using System.Linq;
using Persistance.Model.Payments;
using Shared.Model;

namespace Persistance.Repositories.Impl
{
    public class DepositRepository : IDepositRepository
    {
        private readonly DataContext _context;

        public DepositRepository(DataContext context)
        {
            _context = context;
        }

        public void Add(Deposit deposit)
        {
            _context.Deposits.Add(deposit);
        }

        public void Confirm(long depositId)
        {
            var item = _context.Deposits.Single(q => q.Id == depositId);
            if (item.Status == TranStatus.Confirmed) throw new ArgumentException("Deposit is a confirmed state already.");
            item.Status = TranStatus.Confirmed;
        }

        public void Fail(long depositId)
        {
            var item = _context.Deposits.Single(q => q.Id == depositId);
            if (item.Status == TranStatus.Failed) throw new ArgumentException("Deposit is a failed state already.");
            item.Status = TranStatus.Failed;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
