using System.Collections.Generic;
using Persistance.Model.Accounts;
using Persistance.Model.Payments;

namespace WebApi.Models
{
    public class WithdrawModel
    {
        public UserAccount UserAccount { get; set; }
        public List<UserWithdraw> Withdraws { get; set; }
    }
}