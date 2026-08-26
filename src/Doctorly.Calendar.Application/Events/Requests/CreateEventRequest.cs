using System;
using System.Collections.Generic;
using System.Text;

namespace Doctorly.Calendar.Application.Events.Requests
{
    public record CreateEventRequest(string Title, string Description, DateTimeOffset StartTimeUtc, DateTimeOffset EndTimeUtc, IReadOnlyList<AttendeeInput> Attendees, bool EnforceAvailability = false);
}
