using Doctorly.Calendar.Application.Abstractions;
using Doctorly.Calendar.Application.Common;
using Doctorly.Calendar.Application.Events.Requests;
using Doctorly.Calendar.Application.Events.Responses;
using Doctorly.Calendar.Domain.Common;
using Doctorly.Calendar.Domain.Events;

namespace Doctorly.Calendar.Application.Events;

public class EventService( IEventRepository repository, IUnitOfWork unitOfWork, IOutbox outbox)
{
    public async Task<EventResponse> CreateAsync(CreateEventRequest request, CancellationToken ct)
    {
        if (request.EnforceAvailability &&
            await repository.HasConflictAsync(
                request.Attendees.Select(x => x.Email),
                request.StartTimeUtc,
                request.EndTimeUtc,
                null,
                ct))
        {
            throw new DomainException(
                "One or more attendees are unavailable.");
        }

        var calendarEvent = CalendarEvent.Create(
            request.Title,
            request.Description,
            request.StartTimeUtc,
            request.EndTimeUtc,
            request.Attendees.Select(x => (x.Name, x.Email)));

        await repository.AddAsync(calendarEvent, ct);

        outbox.Add(
            "calendar.event.created",
            Payload(calendarEvent));

        await unitOfWork.SaveChangesAsync(ct);

        return Map(calendarEvent);
    }

    public async Task<EventResponse> GetAsync(Guid id, CancellationToken ct)
    {
        var calendarEvent = await GetRequiredEvent(
            id,
            tracking: false,
            ct);

        return Map(calendarEvent);
    }

    public async Task<EventPage> SearchAsync(EventSearch search, CancellationToken ct)
    {
        search = search with
        {
            Page = Math.Max(1, search.Page),
            PageSize = Math.Clamp(search.PageSize, 1, 100),
            Query = search.Query?.Trim(),
            AttendeeEmail = search.AttendeeEmail?.Trim()
        };

        var result = await repository.SearchAsync(
            search,
            ct);

        return new EventPage(
            result.Items.Select(Map).ToList(),
            result.Page,
            result.PageSize,
            result.TotalCount);
    }

    public async Task<EventResponse> UpdateAsync(Guid id, UpdateEventRequest request, CancellationToken ct)
    {
        if (request.EnforceAvailability &&
            await repository.HasConflictAsync(
                request.Attendees.Select(x => x.Email),
                request.StartTimeUtc,
                request.EndTimeUtc,
                id,
                ct))
        {
            throw new DomainException(
                "One or more attendees are unavailable.");
        }

        var calendarEvent = await GetRequiredEvent(
            id,
            tracking: true,
            ct);

        calendarEvent.Update(
            request.Title,
            request.Description,
            request.StartTimeUtc,
            request.EndTimeUtc,
            request.Attendees.Select(x => (x.Name, x.Email)),
            request.ExpectedVersion);

        outbox.Add(
            "calendar.event.updated",
            Payload(calendarEvent));

        await unitOfWork.SaveChangesAsync(ct);

        return Map(calendarEvent);
    }

    public async Task<EventResponse> CancelAsync(Guid id, long expectedVersion, CancellationToken ct)
    {
        var calendarEvent = await GetRequiredEvent(
            id,
            tracking: true,
            ct);

        calendarEvent.Cancel(expectedVersion);

        outbox.Add(
            "calendar.event.cancelled",
            Payload(calendarEvent));

        await unitOfWork.SaveChangesAsync(ct);

        return Map(calendarEvent);
    }

    public async Task<EventResponse> RespondAsync(Guid id, Guid attendeeId, AttendanceResponseRequest request, CancellationToken ct)
    {
        var calendarEvent = await GetRequiredEvent(
            id,
            tracking: true,
            ct);

        calendarEvent.Respond(
            attendeeId,
            request.Response,
            request.ExpectedVersion);

        outbox.Add(
            "calendar.attendee.responded",
            Payload(calendarEvent));

        await unitOfWork.SaveChangesAsync(ct);

        return Map(calendarEvent);
    }

    public Task<bool> CheckAvailabilityAsync(AvailabilityRequest request, CancellationToken ct)
    {
        return CheckAsync(request, ct);
    }

    private async Task<bool> CheckAsync(AvailabilityRequest request, CancellationToken ct)
    {
        var hasConflict =
            await repository.HasConflictAsync(
                request.Emails,
                request.StartTimeUtc,
                request.EndTimeUtc,
                request.ExcludingEventId,
                ct);

        return !hasConflict;
    }

    private async Task<CalendarEvent> GetRequiredEvent(Guid id, bool tracking, CancellationToken ct)
    {
        return await repository.GetAsync(
                   id,
                   tracking,
                   ct)
               ?? throw new NotFoundException(
                   $"Event '{id}' was not found.");
    }

    private static object Payload(CalendarEvent calendarEvent)
    {
        return new
        {
            calendarEvent.Id,
            calendarEvent.Title,
            calendarEvent.StartTimeUtc,
            calendarEvent.EndTimeUtc,
            Recipients = calendarEvent.Attendees
                .Select(x => x.Email)
                .ToArray()
        };
    }

    private static EventResponse Map(
        CalendarEvent calendarEvent)
    {
        return new EventResponse(
            calendarEvent.Id,
            calendarEvent.Title,
            calendarEvent.Description,
            calendarEvent.StartTimeUtc,
            calendarEvent.EndTimeUtc,
            calendarEvent.Status,
            calendarEvent.Version,
            calendarEvent.CreatedAtUtc,
            calendarEvent.UpdatedAtUtc,
            calendarEvent.Attendees
                .Select(x => new AttendeeResponse(
                    x.Id,
                    x.Name,
                    x.Email,
                    x.Status))
                .ToList());
    }
}

public class NotFoundException(
    string message)
    : Exception(message);