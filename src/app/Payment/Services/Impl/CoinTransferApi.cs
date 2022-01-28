using System.Collections.Generic;
using System.Threading.Tasks;

namespace Payment.Services.Impl
{
    public class CoinTransferApi : IWavesTransferApi
    {
        private readonly ApiRequests _apiRequests;
        private readonly long _confirmations;

        public CoinTransferApi(ApiRequests apiRequests, long confirmations)
        {
            _apiRequests = apiRequests;
            _confirmations = confirmations;
        }
        
        public async Task<long> GetBalanceAsync(string address)
        {
            var json = await _apiRequests.GetRequest($"/addresses/effectiveBalance/{address}/{_confirmations}"); 
            return json.balance; 
        }

        public async Task<TransferResult> TransferAsync(long amount, long fee, string sourceAddress, string targetAddress)
        {
            var body = new Dictionary<string, object>
            {
                {"amount", amount},
                {"fee", fee},
                {"sender", sourceAddress},
                {"recipient", targetAddress}
            };

            var json = await _apiRequests.PostRequest("/assets/transfer", body);

            return new TransferResult
            {
                TransactionId = json.id,
            };
        }
    }
}