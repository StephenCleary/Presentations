namespace TelDemo.WebApi.Services;

public class SqsPublisherService(ILogger<SqsPublisherService> logger)
{
    private static readonly ActivitySource ActivitySource = new("TelDemo.WebApi.SqsPublisher");
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
    private const string QueueName = "teldemo-sqs";
    private const string ServiceUrl = "http://localhost:4566";
    private const string Region = "us-east-1";

    public async Task<string> PublishGenerateWeatherReportMessageAsync()
    {
        using var activity = ActivitySource.StartActivity("PublishSqsMessage", ActivityKind.Producer);
        return await activity.Execute(async () =>
        {
            using var client = new AmazonSQSClient(
                new BasicAWSCredentials("test", "test"),
                new AmazonSQSConfig
                {
                    ServiceURL = ServiceUrl,
                    AuthenticationRegion = Region
                });

            var queueUrl = await GetOrCreateQueueUrlAsync(client);

            // Serialize trace context into SQS message attributes.
            var messageAttributes = new Dictionary<string, MessageAttributeValue>();
            var propagationContext = new PropagationContext(activity?.Context ?? default, Baggage.Current);
            Propagator.Inject(propagationContext, messageAttributes, static (attributes, key, value) =>
            {
                attributes[key] = new MessageAttributeValue
                {
                    DataType = "String",
                    StringValue = value
                };
            });

            var payload = JsonSerializer.Serialize(new
            {
                Message = "Generate weather report",
                SentAtUtc = DateTimeOffset.UtcNow,
            });

            var response = await client.SendMessageAsync(new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = payload,
                MessageAttributes = messageAttributes
            });

            activity?.SetTag("messaging.system", "aws.sqs");
            activity?.SetTag("messaging.destination", QueueName);
            activity?.SetTag("messaging.message.id", response.MessageId);
            logger.LogInformation("Published SQS message {MessageId} to queue {QueueName}", response.MessageId, QueueName);

            return response.MessageId;
        });
    }

    private static async Task<string> GetOrCreateQueueUrlAsync(IAmazonSQS client)
    {
        try
        {
            var queueUrlResponse = await client.GetQueueUrlAsync(QueueName);
            return queueUrlResponse.QueueUrl;
        }
        catch (QueueDoesNotExistException)
        {
            var createQueueResponse = await client.CreateQueueAsync(new CreateQueueRequest
            {
                QueueName = QueueName
            });

            return createQueueResponse.QueueUrl;
        }
    }
}
