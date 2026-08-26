using Doctorly.Calendar.Application.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Doctorly.Calendar.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<EventService>();

        return services;
    }
}