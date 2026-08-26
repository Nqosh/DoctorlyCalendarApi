using Doctorly.Calendar.Application.Abstractions;
using Doctorly.Calendar.Application.Common;
using Doctorly.Calendar.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Doctorly.Calendar.Infrastructure.Persistence;

public sealed class EventRepository(
    CalendarDbContext db)
    : IEventRepository
{
    public Task<CalendarEvent?> GetAsync(Guid id, bool tracking, CancellationToken ct)
    {
        IQueryable<CalendarEvent> query =
            db.Events
                .Include(x => x.Attendees);

        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return query.SingleOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task AddAsync(CalendarEvent item, CancellationToken ct)
    {
        return db.Events.AddAsync(item, ct).AsTask();
    }

    public async Task<PagedResult<CalendarEvent>> SearchAsync(EventSearch search, CancellationToken ct)
    {
        var query = db.Events
            .AsNoTracking()
            .Include(x => x.Attendees)
            .AsSplitQuery()
            .AsQueryable();

        if (search.FromUtc.HasValue)
        {
            query = query.Where(
                x => x.EndTimeUtc >= search.FromUtc.Value);
        }

        if (search.ToUtc.HasValue)
        {
            query = query.Where(
                x => x.StartTimeUtc <= search.ToUtc.Value);
        }

        if (search.Status.HasValue)
        {
            query = query.Where(
                x => x.Status == search.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search.Query))
        {
            query = query.Where(
                x =>
                    x.Title.Contains(search.Query) ||
                    x.Description.Contains(search.Query));
        }

        if (!string.IsNullOrWhiteSpace(search.AttendeeEmail))
        {
            var attendeeEmail =
                search.AttendeeEmail.ToLowerInvariant();

            query = query.Where(
                x => x.Attendees.Any(
                    attendee => attendee.Email == attendeeEmail));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(x => x.StartTimeUtc)
            .Skip((search.Page - 1) * search.PageSize)
            .Take(search.PageSize)
            .ToListAsync(ct);

        return new PagedResult<CalendarEvent>(
            items,
            search.Page,
            search.PageSize,
            totalCount);
    }

    public async Task<bool> HasConflictAsync(IEnumerable<string> emails, DateTimeOffset start, DateTimeOffset end, Guid? excludingId, CancellationToken ct)
    {
        var normalizedEmails = emails
            .Where(email =>
                !string.IsNullOrWhiteSpace(email))
            .Select(email =>
                email.Trim().ToLowerInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (normalizedEmails.Length == 0)
        {
            return false;
        }

        IQueryable<CalendarEvent> events =
            db.Events
                .AsNoTracking()
                .Where(calendarEvent =>
                    calendarEvent.Status ==
                        CalendarEventStatus.Scheduled &&
                    calendarEvent.StartTimeUtc < end &&
                    calendarEvent.EndTimeUtc > start);

        if (excludingId.HasValue)
        {
            events = events.Where(
                calendarEvent =>
                    calendarEvent.Id != excludingId.Value);
        }

        return await events
            .SelectMany(calendarEvent =>
                calendarEvent.Attendees)
            .AnyAsync(
                attendee =>
                    normalizedEmails.Contains(
                        attendee.Email),
                ct);
    }
}