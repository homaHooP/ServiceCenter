namespace ServiceCenter.Messaging;

public record ServiceRequestCreatedMessage(Guid Id, string Title, string RequesterName, string Priority, DateTime CreatedAt);

public class SqsOptions
{
    public string QueueUrl { get; set; } = string.Empty;
}