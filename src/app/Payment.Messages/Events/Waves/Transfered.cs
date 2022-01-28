namespace Payment.Messages.Events.Waves
{
    public class Transfered : WavesEvent
    {
        public long TransactionHeight { get; }
        public string TransactionSignature { get; }
        public long Amount { get; set; }

        public Transfered(object payload, long amount, long transactionHeight, string transactionSignature) : base(payload)
        {
            TransactionHeight = transactionHeight;
            TransactionSignature = transactionSignature;
            Amount = amount;
        }
    }
}