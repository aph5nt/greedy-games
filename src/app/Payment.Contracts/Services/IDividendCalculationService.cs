using Shared.Model;

namespace Payment.Contracts.Services
{
    public interface IDividendCalculationService
    {
        (long spendable, long profit, long dividend) Calculate(Network network, string bankAddress);
    }
}