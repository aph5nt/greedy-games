using Akka.Actor;
using Payment.Contracts.Commands.Waves;
using Payment.Contracts.Events.Waves;
using Payment.Services;
using Shared.Configuration;

namespace Payment.Actors
{

    public class WavesGatewayActor : UntypedActor
    {
        public IWavesApiFactory ApiFactory { get; set; }
        
        public AppServerSettings AppServerSettings { get; set; }
         
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetEffectiveBalance command:
                    var factory = ApiFactory.Create(command.Network);
                    var effectiveBalance = factory.GetBalanceAsync(command.Address).Result;
                    command.Target.Tell(new EffectiveBalance(command.Payload, effectiveBalance));
                    break;

                case GetTransactionConfirmations command:
                    var confirmations = ApiFactory.Create(command.Network).GetTransactionConfirmationstAsync(command.TransactionId).Result;
                    command.Target.Tell(new TransactionInfo(command.Payload, confirmations.Confirmations));
                    break;

                case CreateAddress command:
                    var address = ApiFactory.Create(command.Network).CreateAddressAsync().Result;
                    if (command.Target != null)
                    {
                        command.Target.Tell(new AddressCreated(command.Payload, address));
                    }
                    else
                    {
                        Context.Sender.Tell(new AddressCreated(command.Payload, address));
                    }
                    
                    break;

                case Transfer command:
                    var result = ApiFactory.Create(command.Network)
                        .TransferAsync(
                            command.Amount,
                            command.Fee,
                            command.SourceAddress,
                            command.TargetAddress
                        ).Result;

                    command.Target.Forward(new Transfered(command.Payload, command.Amount, result.TransactionId));
                    
                    break;
            }
        }
    }
}
