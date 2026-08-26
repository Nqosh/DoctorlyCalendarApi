using Doctorly.Calendar.Infrastructure.Notifications;

public class EmailNotificationSender : INotificationSender
{
    public async Task SendAsync(
        Guid messageId,
        string type,
        string payload,
        CancellationToken ct)
    {
        // SMTP or SendGrid logic
    }
}
