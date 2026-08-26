using Doctorly.Calendar.Domain.Common;
using Doctorly.Calendar.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Doctorly.Calendar.Domain.Tests
{
    public class AttendeeResponseTests
    {
        [Fact]
        public void Reject_invalid_email()
        {
            Assert.Throws<DomainException>(
                () => new Attendee(
                    "Alex",
                    "invalid-email"));
        }
    }
}
