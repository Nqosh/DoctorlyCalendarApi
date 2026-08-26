using Doctorly.Calendar.Application.Abstractions;
using Doctorly.Calendar.Application.Common;
using Doctorly.Calendar.Application.Events;
using Doctorly.Calendar.Application.Events.Requests;
using Doctorly.Calendar.Application.Events.Responses;
using Doctorly.Calendar.Domain.Events;
using Microsoft.AspNetCore.Mvc;

namespace Doctorly.Calendar.Api.Controllers;

[ApiController]
[Route("api/v1/events")]

//[Produces("application/json")]
//[Tags("Calendar Events")]
public class EventsController(EventService service) : ControllerBase
{

    [HttpPost]
    [ProducesResponseType<EventResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<EventResponse>> Create(
        CreateEventRequest request,
        CancellationToken ct)
    {
        var result = await service.CreateAsync(request, ct);

        return CreatedAtAction(
            nameof(Get),
            new { id = result.Id },
            result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<EventResponse>(StatusCodes.Status200OK)]
    public Task<EventResponse> Get(Guid id, CancellationToken ct)
    {
        return service.GetAsync(id, ct);
    }

    [HttpGet]
    [ProducesResponseType<EventPage>(StatusCodes.Status200OK)]
    public Task<EventPage> Search(
        [FromQuery] DateTimeOffset? fromUtc,
        [FromQuery] DateTimeOffset? toUtc,
        [FromQuery] string? query,
        [FromQuery] string? attendeeEmail,
        [FromQuery] CalendarEventStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        return service.SearchAsync(
            new EventSearch(
                fromUtc,
                toUtc,
                query,
                attendeeEmail,
                status,
                page,
                pageSize),
            ct);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<EventResponse>(StatusCodes.Status200OK)]
    public Task<EventResponse> Update(Guid id, UpdateEventRequest request, CancellationToken ct)
    {
        return service.UpdateAsync(id, request, ct);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType<EventResponse>(StatusCodes.Status200OK)]
    public Task<EventResponse> Cancel(Guid id, [FromQuery] long expectedVersion, CancellationToken ct)
    {
        return service.CancelAsync(
            id,
            expectedVersion,
            ct);
    }

    [HttpPut("{eventId:guid}/attendees/{attendeeId:guid}/response")]
    [ProducesResponseType<EventResponse>(StatusCodes.Status200OK)]
    public Task<EventResponse> Respond(Guid eventId,  Guid attendeeId, AttendanceResponseRequest request,CancellationToken ct)
    {
        return service.RespondAsync(
            eventId,
            attendeeId,
            request,
            ct);
    }

    [HttpPost("availability")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    public Task<bool> Availability(AvailabilityRequest request, CancellationToken ct)
    {
        return service.CheckAvailabilityAsync(
            request,
            ct);
    }
}