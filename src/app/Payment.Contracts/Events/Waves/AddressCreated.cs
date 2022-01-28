namespace Payment.Contracts.Events.Waves
{
    public class AddressCreated : WavesEvent
    {
        public string Address { get; }

        public AddressCreated(object payload, string address) : base(payload)
        {
            Address = address;
        }
    }
}