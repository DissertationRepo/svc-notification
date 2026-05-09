namespace NotificationService.Infrastructure.Messaging
{
    public sealed class KafkaSettings
    {
        public string BootstrapServers { get; init; } = "localhost:9092";
        public string GroupId { get; init; } = "svc-notification";
        public string MessageSentTopic { get; init; } = "messages.sent";
        public string AutoOffsetReset { get; init; } = "Earliest";
        public bool EnableAutoCommit { get; init; } = false;
    }
}
