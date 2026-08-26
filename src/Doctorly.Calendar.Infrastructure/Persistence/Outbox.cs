using System.Globalization;
using System.Text.Json;
using Doctorly.Calendar.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Doctorly.Calendar.Infrastructure.Persistence;

public class Outbox(CalendarDbContext db) : IOutbox
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public void Add(string type, object payload)
    {
        db.OutboxMessages.Add(
            new OutboxMessage
            {
                Type = type,
                Payload = JsonSerializer.Serialize(
                    payload,
                    JsonOptions)
            });
    }
}

