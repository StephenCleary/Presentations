using System.Text;
using System.Text.Json;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;

namespace TelDemo.Frontend.Services;

public class RabbitMqPublisherService(ILogger<RabbitMqPublisherService> logger)
{
    private static readonly ActivitySource ActivitySource = new("TelDemo.Frontend.Publisher");
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
    private const string QueueName = "teldemo";

    public async Task<string> PublishGenerateWeatherReportMessageAsync()
    {
        using var activity = ActivitySource.StartActivity("PublishMessage", ActivityKind.Producer);
        return await activity.Execute(async () =>
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

            var propagationContext = new PropagationContext(activity?.Context ?? default, Baggage.Current);
            Propagator.Inject(propagationContext, properties, static (props, key, value) =>
            {
                props.Headers ??= new Dictionary<string, object?>();
                props.Headers[key] = value;
            });

            var body = Encoding.UTF8.GetBytes(payload);
            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: QueueName, mandatory: false, basicProperties: properties, body: body);

            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.destination", QueueName);
            activity?.SetTag("messaging.message.id", messageId);
            logger.LogInformation("Published RabbitMQ message {MessageId} to queue {QueueName}", messageId, QueueName);

            return messageId;
        });
    }
}
