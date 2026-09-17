namespace _9_Outbox.Events
{
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal Amount { get; set; }
    }
}
