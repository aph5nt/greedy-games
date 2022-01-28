using System;
using System.Threading.Tasks;

namespace Payment.Services
{
    public interface IWavesApi : IDisposable
    {
        Task<string> CreateAddressAsync();
        Task<bool> DeleteAddressAsync(string address);
        Task<long> GetBalanceAsync(string address);
        Task<GetTransactionConfirmationsResult> GetTransactionConfirmationstAsync(string transactionId);
        Task<GetTransactionResult> GetTransactionAsync(string txId);

        Task<TransferResult> TransferAsync(long amount, long fee, string sourceAddress, string targetAddress);

        Task<bool> AddressValidateAsync(string address);
        Task<int> GetBlockHeightAsync();
    }

    public class TransferResult
    {
        public string TransactionId { get; set; }
    }

    public class GetTransactionResult : TransferResult
    {
        public long Amount { get; set; }
    }

    public class GetTransactionConfirmationsResult
    {
        public long Confirmations { get; set; }

    }
}