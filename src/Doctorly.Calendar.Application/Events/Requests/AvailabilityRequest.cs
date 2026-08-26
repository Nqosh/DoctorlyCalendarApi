using System;
using System.Collections.Generic;
using System.Text;

namespace Doctorly.Calendar.Application.Events.Requests
{
    public record AvailabilityRequest(IReadOnlyList<string> Emails, DateTimeOffset StartTimeUtc, DateTimeOffset EndTimeUtc, Guid? ExcludingEventId = null);
}
