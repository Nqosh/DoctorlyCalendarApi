using Microsoft.Extensions.Logging;

namespace Doctorly.Calendar.Infrastructure.Notifications;

public class LoggingNotificationSender(
    ILogger<LoggingNotificationSender> logger)
    : INotificationSender
{
    public Task SendAsync(
        Guid messageId,
        string type,
        string payload,
        CancellationToken ct)
    {
        logger.LogInformation(
            "Notification {MessageId} of type {Type} queued. Payload length: {PayloadLength}",
            messageId,
            type,
            payload.Length);

        return Task.CompletedTask;
    }
}