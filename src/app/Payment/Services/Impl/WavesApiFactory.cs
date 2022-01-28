using Shared.Configuration;
using Shared.Model;
using System;

namespace Payment.Services.Impl
{
    public class WavesApiFactory : IWavesApiFactory
    {
        public AppServerSettings Settings { get; set; }

        public IWavesApi Create(Network network)
        {
            if(network == Network.FREE)
                throw new NotSupportedException();

            var api = new ApiRequests(new Uri(Settings.Waves.Node.Url), Settings.Waves.Node.ApiKey);
            
            return network == Network.WAVES ?
                new WavesApi(new CoinTransferApi(api, Settings.Waves.Transaction.Confirmations), api) :
                new WavesApi(new AssetTransferApi(api, Settings.Payments.GetBy(network).AssetId), api);
        }
    }
}