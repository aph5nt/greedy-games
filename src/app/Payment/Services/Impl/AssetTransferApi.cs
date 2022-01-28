using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Payment.Services.Impl
{
    public class AssetTransferApi : IWavesTransferApi
    {
        private readonly ApiRequests _apiRequests;
        private readonly string _assetId;

        public AssetTransferApi(ApiRequests apiRequests, string assetId)
        {
            _apiRequests = apiRequests;
            _assetId = assetId;
        }
        
        public async Task<long> GetBalanceAsync(string address)
        {
            var json = await _apiRequests.GetRequest($"/assets/balance/{address}/{_assetId}"); 
            return json.balance; 
        }

        public async Task<TransferResult> TransferAsync(long amount, long fee, string sourceAddress, string targetAddress)
        {
            var body = new Dictionary<string, object>
            {
                {"assetId", _assetId},
                {"amount", amount},
                {"fee", fee},
                { "feeAssetId", _assetId},
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