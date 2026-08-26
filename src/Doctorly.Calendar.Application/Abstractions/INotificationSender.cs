namespace Doctorly.Calendar.Infrastructure.Notifications;

public interface INotificationSender
{
    Task SendAsync(Guid messageId, string type, string payload, CancellationToken ct);
}
