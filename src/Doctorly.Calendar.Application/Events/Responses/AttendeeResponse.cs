using Doctorly.Calendar.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Doctorly.Calendar.Application.Events.Responses
{
    public record AttendeeResponse(Guid Id, string Name, string Email, AttendanceStatus Status);
}
