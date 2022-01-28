using System.Collections.Generic;
using System.Linq;
using Shared.Model;

namespace Shared.Configuration
{

    public class Payment
    {
        public Network Network { get; set; }
        public decimal ProfitRatio { get; set; }
        public string BankAddress { get; set; }
        public string BankDepositAddress { get; set; }
        public string DividendAddress { get; set; }
        public string ProfitAddress { get; set; }
        
        public string AssetId { get; set; }
    }
    
    public class Payments
    {
        public List<Payment> Networks { get; set; }

        public Payment GetBy(Network network)
        {
            return Networks.Single(x => x.Network == network);
        }
    }
}