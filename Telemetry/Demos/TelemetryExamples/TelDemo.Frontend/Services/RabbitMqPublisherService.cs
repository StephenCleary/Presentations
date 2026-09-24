namespace TelDemo.Frontend.Services;

public class RabbitMqPublisherService(ILogger<RabbitMqPublisherService> logger)
{
    private const string QueueName = "teldemo";

    public async Task<string> PublishGenerateWeatherReportMessageAsync()
    {
        var messageId = Guid.NewGuid().ToString("N");

        var payload = JsonSerializer.Serialize(new
        {
            Message = "Generate weather report",
            SentAtUtc = DateTimeOffset.UtcNow,
        });

        var factory = new ConnectionFactory { HostName = "localhost" };
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var properties = new BasicProperties
        {
            MessageId = messageId,
            ContentType = "application/json",
            Headers = new Dictionary<string, object?>()
        };

        var body = Encoding.UTF8.GetBytes(payload);
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: QueueName, mandatory: false, basicProperties: properties, body: body);

        logger.LogInformation("Published RabbitMQ message {MessageId} to queue {QueueName}", messageId, QueueName);

        return messageId;
    }
}
