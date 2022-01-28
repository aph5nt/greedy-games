using System.Threading.Tasks;
using Shared.Configuration;

namespace Payment.Services.Impl
{
    public class WavesApi : IWavesApi
    {
        private readonly IWavesTransferApi _transferApi;
        private readonly ApiRequests _apiRequests;
        
        public WavesApi(IWavesTransferApi transferApi, ApiRequests apiRequests)
        {
            _transferApi = transferApi;
            _apiRequests = apiRequests;
        }

        public async Task<string> CreateAddressAsync()
        {
            var json = await _apiRequests.PostRequest("/addresses", "{}");
            return json.address;
        }

        public async Task<bool> DeleteAddressAsync(string address)
        {
            return await _apiRequests.DeleteAddressAsync($"/addresses/{address}");
        }

        public async Task<TransferResult> TransferAsync(long amount, long fee, string sourceAddress, string targetAddress)
        {
            return await _transferApi.TransferAsync(amount, fee, sourceAddress, targetAddress);
        }

        public async Task<long> GetBalanceAsync(string address)
        {
            return await _transferApi.GetBalanceAsync(address);
        }

        public async Task<int> GetBlockHeightAsync()
        {
            var json = await _apiRequests.GetRequest("/blocks/height");
            return json.height;
        }

        public async Task<bool> AddressValidateAsync(string address)
        {
            var json = await _apiRequests.GetRequest($"/addresses/validate/{address}");
            return json.valid;
        }

        public async Task<GetTransactionResult> GetTransactionAsync(string txId)
        {
            var json = await _apiRequests.GetRequest($"/transactions/info/{txId}");

            return new GetTransactionResult
            {
                TransactionId = json.id,
                Amount = json.amount
            };
        }

        public async Task<GetTransactionConfirmationsResult> GetTransactionConfirmationstAsync(string txid)
        {
            try
            {
                // confirmations = last block height - transaction block height + 1
                var tx = await _apiRequests.GetRequest($"/transactions/info/{txid}");
                var block = await _apiRequests.GetRequest("/blocks/height");
                return new GetTransactionConfirmationsResult
                {
                    Confirmations = block.height - tx.height + 1
                };
            }
            catch
            {
                return new GetTransactionConfirmationsResult
                {
                    Confirmations = -1
                };
            }
        }

        public void Dispose()
        {
            _apiRequests?.Dispose();
        }
    }
}