using System.Threading.Tasks;

namespace Payment.Services
{
    public interface IWavesTransferApi
    {
        Task<long> GetBalanceAsync(string address);

        Task<TransferResult> TransferAsync(long amount, long fee, string sourceAddress, string targetAddress);
    }
}