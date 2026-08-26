using Doctorly.Calendar.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Doctorly.Calendar.Application.Common
{
    public record EventSearch(DateTimeOffset? FromUtc, DateTimeOffset? ToUtc, string? Query, string? AttendeeEmail, CalendarEventStatus? Status, int Page, int PageSize);
}
