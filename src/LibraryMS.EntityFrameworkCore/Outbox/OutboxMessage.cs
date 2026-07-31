namespace LibraryMS.EntityFrameworkCore.Outbox;

// Represents a domain event stored in the Outbox queue table.
// Implements the Transactional Outbox Pattern with Retry Mechanism.
public sealed class OutboxMessage
{
    public Guid Id { get; private set; }

    // The fully qualified type name of the domain event.
    public string Type { get; private set; } = default!;

    // JSON-serialized domain event payload.
    public string Content { get; private set; } = default!;

    // UTC timestamp when the domain event occurred.
    public DateTime OccurredOn { get; private set; }

    // UTC timestamp when the message was successfully processed. Null = unprocessed.
    public DateTime? ProcessedOn { get; private set; }

    // Last error message from a failed processing attempt.
    public string? Error { get; private set; }

    // How many times processing has been attempted.
    public int RetryCount { get; private set; }

    // Max number of retries before the message is considered dead.
    public int MaxRetries { get; private set; } = 3;

    // Optional category for grouping messages (e.g. for Chain of Responsibility).
    public string? Category { get; private set; }

    // Optional scheduled time for the message to be processed.
    public DateTime? ScheduledFor { get; private set; }

    private OutboxMessage() { }

    public static OutboxMessage Create(string type, string content)
        => new()
        {
            Id = Guid.NewGuid(),
            Type = type,
            Content = content,
            OccurredOn = DateTime.UtcNow,
            RetryCount = 0,
            MaxRetries = 3
        };

    public static OutboxMessage CreateScheduled(string type, string content, string category, DateTime scheduledFor)
        => new()
        {
            Id = Guid.NewGuid(),
            Type = type,
            Content = content,
            OccurredOn = DateTime.UtcNow,
            RetryCount = 0,
            MaxRetries = 3,
            Category = category,
            ScheduledFor = scheduledFor
        };

    // Marks the message as successfully processed.
    public void MarkAsProcessed()
    {
        ProcessedOn = DateTime.UtcNow;
        Error = null;
    }

    // Increments retry count and records the error message.
    public void RecordFailure(string error)
    {
        RetryCount++;
        Error = error;
    }

    // Returns true when the message has exceeded max retry limit.
    public bool IsDeadLetter => RetryCount >= MaxRetries && ProcessedOn is null;

    // Returns true when the message is eligible for processing.
    public bool IsEligibleForProcessing => ProcessedOn is null && RetryCount < MaxRetries && (ScheduledFor is null || ScheduledFor <= DateTime.UtcNow);
}
