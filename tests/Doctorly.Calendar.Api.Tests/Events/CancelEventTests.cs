using Doctorly.Calendar.Application.Events.Requests;
using Doctorly.Calendar.Application.Events.Responses;
using Doctorly.Calendar.Domain.Events;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Xunit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Doctorly.Calendar.Api.Tests.Events
{
    public class CancelEventTests(TestFactory factory) : IClassFixture<TestFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task Cancel_event_marks_event_as_cancelled()
        {
            var eventResponse = await CreateEvent();

            var response = await _client.DeleteAsync(
                $"/api/v1/events/{eventResponse.Id}" +
                $"?expectedVersion={eventResponse.Version}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var cancelled =
                await response.Content
                    .ReadFromJsonAsync<EventResponse>();

            Assert.Equal(CalendarEventStatus.Cancelled, cancelled!.Status);
        }

        private async Task<EventResponse> CreateEvent()
        {
            var start = DateTimeOffset.UtcNow;

            var response = await _client.PostAsJsonAsync(
                "/api/v1/events",
                new CreateEventRequest(
                    "Check-up",
                    "Routine",
                    start,
                    start.AddMinutes(30),
                    [
                        new AttendeeInput(
                    "Alex",
                    "alex@test.dev")
                    ]));

            response.EnsureSuccessStatusCode();

            return (await response.Content
                .ReadFromJsonAsync<EventResponse>())!;
        }
    }
}
