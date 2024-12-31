namespace Shared.Core.Messaging;

public class MessagingOptions
{
    public BrokerType BrokerType { get; set; } = BrokerType.InMemory;
    public RabbitMQOptions RabbitMQOptions { get; set; } = default!;
    public KafkaOptions KafkaOptions { get; set; } = default!;
}

public class RabbitMQOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
}

public class KafkaOptions
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string Topic { get; set; } = "default-topic";
    public string GroupId { get; set; } = "default-group";
    public string ClientId { get; set; } = "default-client";
}

public enum BrokerType
{
    InMemory,
    Kafka,
    RabbitMQ,
}
