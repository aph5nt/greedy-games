using System.Collections.Generic;
using Persistance.Model.Accounts;
using Persistance.Model.Payments;

namespace Web.Models
{
    public class DepositViewModel
    {
        public UserAccount UserAccount { get; set; }
        public List<Deposit>    Deposits { get; set; }
    }
}