using System;
using System.Collections.Generic;
using System.Text;

namespace Doctorly.Calendar.Application.Events.Responses
{
    public record EventPage(IReadOnlyList<EventResponse> Items, int Page, int PageSize, int TotalCount);
}
