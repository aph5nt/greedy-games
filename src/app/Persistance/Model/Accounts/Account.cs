using System;
using Shared.Model;

namespace Persistance.Model.Accounts
{
    public class Account : Identity
    {
        public string DepositAddress { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}