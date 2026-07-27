namespace LibraryMS.EntityFrameworkCore.Outbox;

/// <summary>
/// Represents a domain event stored in the Outbox queue table.
/// Implements the Transactional Outbox Pattern with Retry Mechanism.
/// </summary>
public sealed class OutboxMessage
{
    public Guid Id { get; private set; }

    /// <summary>The fully qualified type name of the domain event.</summary>
    public string Type { get; private set; } = default!;

    /// <summary>JSON-serialized domain event payload.</summary>
    public string Content { get; private set; } = default!;

    /// <summary>UTC timestamp when the domain event occurred.</summary>
    public DateTime OccurredOn { get; private set; }

    /// <summary>UTC timestamp when the message was successfully processed. Null = unprocessed.</summary>
    public DateTime? ProcessedOn { get; private set; }

    /// <summary>Last error message from a failed processing attempt.</summary>
    public string? Error { get; private set; }

    /// <summary>How many times processing has been attempted.</summary>
    public int RetryCount { get; private set; }

    /// <summary>Max number of retries before the message is considered dead.</summary>
    public int MaxRetries { get; private set; } = 3;

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

    /// <summary>Marks the message as successfully processed.</summary>
    public void MarkAsProcessed()
    {
        ProcessedOn = DateTime.UtcNow;
        Error = null;
    }

    /// <summary>Increments retry count and records the error message.</summary>
    public void RecordFailure(string error)
    {
        RetryCount++;
        Error = error;
    }

    /// <summary>Returns true when the message has exceeded max retry limit.</summary>
    public bool IsDeadLetter => RetryCount >= MaxRetries && ProcessedOn is null;

    /// <summary>Returns true when the message is eligible for processing.</summary>
    public bool IsEligibleForProcessing => ProcessedOn is null && RetryCount < MaxRetries;
}
