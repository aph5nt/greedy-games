using Shared.Model;

namespace Payment.Services
{
    public interface IWavesApiFactory
    {
        IWavesApi Create(Network network);
    }
}