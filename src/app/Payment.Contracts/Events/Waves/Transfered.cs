namespace Payment.Contracts.Events.Waves
{
    public class Transfered : WavesEvent
    {
        public string TransactionId { get; }
        public long Amount { get; }

        public Transfered(object payload, long amount, string transactionId) : base(payload)
        {
            TransactionId = transactionId;
            Amount = amount;
        }
    }
}