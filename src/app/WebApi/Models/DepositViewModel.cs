using System.Collections.Generic;
using Persistance.Model.Accounts;
using Persistance.Model.Payments;

namespace WebApi.Models
{
    public class DepositViewModel
    {
        public UserAccount UserAccount { get; set; }
        public List<Deposit>    Deposits { get; set; }
    }
}