using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;

namespace ServiceCenter.Messaging;

public class ServiceRequestQueueWorker(IAmazonSQS sqs, IOptions<SqsOptions> options, ILogger<ServiceRequestQueueWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var queueUrl = options.Value.QueueUrl;

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var response = await sqs.ReceiveMessageAsync(new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 20 // long polling
                }, ct);

                foreach (var msg in response.Messages ?? [])
                {
                    var payload = JsonSerializer.Deserialize<ServiceRequestCreatedMessage>(msg.Body);
                    logger.LogInformation("New request {Id}: {Title} ({Priority})",
                        payload?.Id, payload?.Title, payload?.Priority);

                    await sqs.DeleteMessageAsync(queueUrl, msg.ReceiptHandle, ct);
                }
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SQS polling failed");
                await Task.Delay(TimeSpan.FromSeconds(5), ct);
            }
        }
    }
}