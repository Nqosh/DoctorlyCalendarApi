using Doctorly.Calendar.Application.Events.Requests;
using Doctorly.Calendar.Application.Events.Responses;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Xunit;

namespace Doctorly.Calendar.Api.Tests.Events
{
    public class SearchEventTests(TestFactory factory) : IClassFixture<TestFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task Search_returns_matching_events()
        {
            await CreateEvent();

            var response =
                await _client.GetAsync(
                    "/api/v1/events?page=1&pageSize=10");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var page =
                await response.Content
                    .ReadFromJsonAsync<EventPage>();

            Assert.NotEmpty(page!.Items);
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
