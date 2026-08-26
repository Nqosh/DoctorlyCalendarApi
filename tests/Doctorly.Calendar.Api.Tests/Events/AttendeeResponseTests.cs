using Doctorly.Calendar.Application.Events.Requests;
using Doctorly.Calendar.Application.Events.Responses;
using Doctorly.Calendar.Domain.Events;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Xunit;

namespace Doctorly.Calendar.Api.Tests.Events
{
    public class AttendeeResponseTests(TestFactory factory) : IClassFixture<TestFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();
        [Fact]
        public async Task Attendee_can_accept_event()
        {
            var eventResponse =
                await CreateEvent();

            var attendee =
                eventResponse.Attendees.First();

            var response =
                await _client.PutAsJsonAsync(
                    $"/api/v1/events/{eventResponse.Id}" +
                    $"/attendees/{attendee.Id}/response",
                    new AttendanceResponseRequest(
                        AttendanceStatus.Accepted,
                        eventResponse.Version));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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
