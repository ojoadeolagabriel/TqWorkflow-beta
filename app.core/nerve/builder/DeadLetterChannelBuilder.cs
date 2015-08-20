namespace app.core.nerve.builder
{
    public class DeadLetterChannelBuilder
    {
        public string DeadLetterUri { get; set; }
        public int MaxDeliveryAttempts { get; set; }
    }
}
