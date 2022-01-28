namespace Payment.Contracts.Events.Waves
{
    public class EffectiveBalance : WavesEvent
    {
        public long Amount { get; set; }

        public EffectiveBalance(object payload, long amount) : base(payload)
        {
            Amount = amount;
        }
    }
}