using Doctorly.Calendar.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Doctorly.Calendar.Application.Events.Requests
{
    public record AttendanceResponseRequest(AttendanceStatus Response, long ExpectedVersion);
}
