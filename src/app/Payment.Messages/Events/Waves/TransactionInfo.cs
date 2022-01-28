namespace Payment.Messages.Events.Waves
{
    public class TransactionInfo : WavesEvent
    {
        public long Confirmations { get; }

        public TransactionInfo(object payload, long confirmations) : base(payload)
        {
            Confirmations = confirmations;
        }
    }
}