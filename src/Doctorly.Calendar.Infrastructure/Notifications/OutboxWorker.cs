using Doctorly.Calendar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Doctorly.Calendar.Infrastructure.Notifications;

public class OutboxWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxWorker> logger)
    : BackgroundService
{
    private static readonly TimeSpan Delay =
        TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DispatchAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Outbox dispatch cycle failed.");
            }

            await Task.Delay(
                Delay,
                stoppingToken);
        }
    }

    private async Task DispatchAsync(CancellationToken ct)
    {
        await using var scope =
            scopeFactory.CreateAsyncScope();

        var db =
            scope.ServiceProvider
                .GetRequiredService<CalendarDbContext>();

        var sender =
            scope.ServiceProvider
                .GetRequiredService<INotificationSender>();

        var messages =
            await db.OutboxMessages
                .Where(x =>
                    x.ProcessedAtUtc == null &&
                    x.Attempts < 5)
                .OrderBy(x => x.OccurredAtUtc)
                .Take(20)
                .ToListAsync(ct);

        foreach (var message in messages)
        {
            try
            {
                await sender.SendAsync(
                    message.Id,
                    message.Type,
                    message.Payload,
                    ct);

                message.ProcessedAtUtc =
                    DateTimeOffset.UtcNow;

                message.LastError = null;
            }
            catch (Exception ex)
            {
                message.Attempts++;

                message.LastError =
                    ex.Message[
                        ..Math.Min(
                            ex.Message.Length,
                            2000)];
            }
        }

        await db.SaveChangesAsync(ct);
    }
}