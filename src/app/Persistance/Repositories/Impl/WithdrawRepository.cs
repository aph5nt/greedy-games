using System;
using System.Linq;
using Persistance.Model.Payments;
using Shared.Model;

namespace Persistance.Repositories.Impl
{
    public class WithdrawRepository : IWithdrawRepository
    {
        private readonly DataContext _context;

        public WithdrawRepository(DataContext context)
        {
            _context = context;
        }
        
        public void Add(Withdraw userWithdraw)
        {
            _context.Withdraws.Add(userWithdraw);
        }

        public void Confirm(long withdrawId)
        {
            var item = _context.Withdraws.Single(q => q.Id == withdrawId);
            if (item.Status == TranStatus.Confirmed) throw new ArgumentException("Withdraw is confirmed already.");
            item.Status = TranStatus.Confirmed;
        }

        public void Fail(long withdrawId)
        {
            var item = _context.Withdraws.Single(q => q.Id == withdrawId);
            if(item.Status == TranStatus.Failed) throw new ArgumentException("Withdraw is failed already.");
            item.Status = TranStatus.Failed;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}