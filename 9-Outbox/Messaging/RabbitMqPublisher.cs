using System.Text;
using RabbitMQ.Client;

namespace OutboxDemo.Messaging;

public class RabbitMqPublisher
{
    private readonly IConfiguration _configuration;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task PublishAsync(
        string messageType,
        string payload)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"],
            Port = int.Parse(_configuration["RabbitMQ:Port"]!),
            UserName = _configuration["RabbitMQ:Username"],
            Password = _configuration["RabbitMQ:Password"]
        };

        await using var connection =
            await factory.CreateConnectionAsync();

        await using var channel =
            await connection.CreateChannelAsync();

        // 1. Create Exchange
        await channel.ExchangeDeclareAsync(
            exchange: "orders",
            type: ExchangeType.Direct,
            durable: true);

        // 2. Create Queue
        await channel.QueueDeclareAsync(
            queue: "order-created",
            durable: true,
            exclusive: false,
            autoDelete: false);

        // 3. Bind Queue to Exchange
        await channel.QueueBindAsync(
            queue: "order-created",
            exchange: "orders",
            routingKey: messageType);

        // 4. Create message body
        var body = Encoding.UTF8.GetBytes(payload);

        // 5. Publish
        await channel.BasicPublishAsync(
            exchange: "orders",
            routingKey: messageType,
            body: body);
    }
}