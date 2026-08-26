using System;
using System.Collections.Generic;
using System.Text;

namespace Doctorly.Calendar.Application.Events.Requests
{
    public record UpdateEventRequest(string Title, string Description, DateTimeOffset StartTimeUtc, DateTimeOffset EndTimeUtc, IReadOnlyList<AttendeeInput> Attendees, long ExpectedVersion, bool EnforceAvailability = false);
}
