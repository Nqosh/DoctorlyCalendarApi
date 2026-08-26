using Doctorly.Calendar.Application.Abstractions;
using Doctorly.Calendar.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Doctorly.Calendar.Infrastructure.Persistence;

public class UnitOfWork(CalendarDbContext db) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken ct)
    {
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException("The event changed after it was loaded. Reload and retry with the current version.");
        }
    }
}