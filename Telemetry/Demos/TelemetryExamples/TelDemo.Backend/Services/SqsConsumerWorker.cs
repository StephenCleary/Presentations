namespace TelDemo.Backend.Services;

public sealed class SqsConsumerWorker(ILogger<SqsConsumerWorker> logger, ReportGeneratorService reportGenerator) : BackgroundService
{
    private static readonly ActivitySource ConsumerActivitySource = new("TelDemo.Backend.Consumer");
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
    private const string QueueName = "teldemo-sqs";
    private const string ServiceUrl = "http://localhost:4566";
    private const string Region = "us-east-1";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var client = new AmazonSQSClient(
            new BasicAWSCredentials("test", "test"),
            new AmazonSQSConfig
            {
                ServiceURL = ServiceUrl,
                AuthenticationRegion = Region
            });

        var queueUrl = await GetOrCreateQueueUrlAsync(client, stoppingToken);
        logger.LogInformation("Consuming SQS queue {QueueName} from {ServiceUrl}", QueueName, ServiceUrl);

        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await client.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 5,
                WaitTimeSeconds = 20,
                MessageAttributeNames = ["All"]
            }, stoppingToken);

            if (response.Messages == null)
                continue;

            foreach (var message in response.Messages)
            {
                using var loggingContext = logger.BeginScope(new Dictionary<string, object?>
                {
                    ["SqsMessageId"] = message.MessageId,
                    ["SqsReceiptHandle"] = message.ReceiptHandle,
                });

                // Deserialize Publisher context from SQS message attributes.
                var messageContext = Propagator.Extract(default, message, ExtractTraceContextFromMessage);
                Baggage.Current = messageContext.Baggage;

                // The following code creates the Consumer activity as a child of the Producer activity.
                // This is fine if your backend consumers are logically children of the producer.
                // For a more loosely-connected architecture, you can create the Consumer activity "linked" to the Producer activity as such:
                // ```
                // using var consumeActivity = ConsumerActivitySource.StartActivity("ProcessSqsMessage", ActivityKind.Consumer, default(ActivityContext),
                //     links: messageContext.ActivityContext == default ? [] : [new ActivityLink(messageContext.ActivityContext)]);
                // ```
                // Note that telemetry visualization backends may or may not support navigation between *linked* activities.

                using var consumeActivity = ConsumerActivitySource.StartActivity("ProcessSqsMessage", ActivityKind.Consumer, messageContext.ActivityContext);
                consumeActivity?.SetTag("messaging.system", "aws.sqs");
                consumeActivity?.SetTag("messaging.destination", QueueName);
                consumeActivity?.SetTag("messaging.message.id", message.MessageId);

                try
                {
                    await consumeActivity.Execute(async () =>
                    {
                        var report = await reportGenerator.GenerateReportAsync(System.Text.Encoding.UTF8.GetBytes(message.Body));
                        await client.DeleteMessageAsync(queueUrl, message.ReceiptHandle, stoppingToken);
                        logger.LogInformation("Consumed SQS message. {Report}", report);
                    });
                }
                catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogError(ex, "Failed to process SQS message {MessageId}. Message will remain in the queue.", message.MessageId);
                }
            }
        }
    }

    private static async Task<string> GetOrCreateQueueUrlAsync(IAmazonSQS client, CancellationToken cancellationToken)
    {
        try
        {
            var queueUrlResponse = await client.GetQueueUrlAsync(QueueName, cancellationToken);
            return queueUrlResponse.QueueUrl;
        }
        catch (QueueDoesNotExistException)
        {
            var createQueueResponse = await client.CreateQueueAsync(new CreateQueueRequest
            {
                QueueName = QueueName
            }, cancellationToken);

            return createQueueResponse.QueueUrl;
        }
    }

    private static IEnumerable<string> ExtractTraceContextFromMessage(Message message, string key)
    {
        var messageAttributes = message.MessageAttributes;
        if (messageAttributes is null || !messageAttributes.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value?.StringValue))
            return [];

        return [value.StringValue];
    }
}
