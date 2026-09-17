namespace _9_Outbox.Models
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }

        public string Type { get; set; } = string.Empty;

        public string Payload { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? ProcessedAt { get; set; }

        public int RetryCount { get; set; }

        public string? Error { get; set; }
    }
}
