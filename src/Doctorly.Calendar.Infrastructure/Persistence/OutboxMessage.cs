namespace Doctorly.Calendar.Infrastructure.Persistence;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTimeOffset OccurredAtUtc { get; set; } =
        DateTimeOffset.UtcNow;

    public string Type { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public DateTimeOffset? ProcessedAtUtc { get; set; }

    public int Attempts { get; set; }

    public string? LastError { get; set; }
}