using Doctorly.Calendar.Application.Abstractions;
using Doctorly.Calendar.Infrastructure.Notifications;
using Doctorly.Calendar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Doctorly.Calendar.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CalendarDbContext>(options =>
            options.UseSqlite(
                configuration.GetConnectionString("Calendar")
                ?? "Data Source=doctorly-calendar.db"));

        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOutbox, Outbox>();
        services.AddScoped<INotificationSender, LoggingNotificationSender>();
        services.AddScoped<INotificationSender, EmailNotificationSender>();
        services.AddHostedService<OutboxWorker>();

        return services;
    }
}