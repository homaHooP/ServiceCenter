using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using ServiceCenter.Models;

namespace ServiceCenter.Messaging;

public class ServiceRequestQueue(IAmazonSQS sqs, IOptions<SqsOptions> options)
{
    public async Task PublishCreatedAsync(ServiceRequest request, CancellationToken ct = default)
    {
        var message = new ServiceRequestCreatedMessage(
            request.Id, request.Title, request.RequesterName,
            request.Priority.ToString(), request.CreatedAt);

        await sqs.SendMessageAsync(new SendMessageRequest
        {
            QueueUrl = options.Value.QueueUrl,
            MessageBody = JsonSerializer.Serialize(message)
        }, ct);
    }
}