using Doctorly.Calendar.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Doctorly.Calendar.Application.Events.Responses
{
    public record EventResponse(Guid Id, string Title, string Description, DateTimeOffset StartTimeUtc, DateTimeOffset EndTimeUtc, CalendarEventStatus Status, long Version, DateTimeOffset CreatedAtUtc, DateTimeOffset UpdatedAtUtc, IReadOnlyList<AttendeeResponse> Attendees);
}
