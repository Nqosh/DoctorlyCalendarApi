using Doctorly.Calendar.Application.Events.Requests;
using Doctorly.Calendar.Application.Events.Responses;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;
using System.Net.Http.Json;

namespace Doctorly.Calendar.Api.Tests.Events
{
    public class UpdateEventTests(TestFactory factory) : IClassFixture<TestFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task Update_event_returns_ok()
        {
            // Arrange
            var start = DateTimeOffset.UtcNow;

            var createResponse = await _client.PostAsJsonAsync(
                "/api/v1/events",
                new CreateEventRequest(
                    "Original",
                    "Description",
                    start,
                    start.AddMinutes(30),
                    [new AttendeeInput("Alex", "alex@test.dev")]));

            var created =
                await createResponse.Content
                    .ReadFromJsonAsync<EventResponse>();

            // Act
            var updateResponse = await _client.PutAsJsonAsync(
                $"/api/v1/events/{created!.Id}",
                new UpdateEventRequest(
                    "Updated",
                    "Updated Description",
                    start,
                    start.AddMinutes(60),
                    [new AttendeeInput("Alex", "alex@test.dev")],
                    created.Version));

            // Assert
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            var updated =
                await updateResponse.Content
                    .ReadFromJsonAsync<EventResponse>();

            Assert.Equal("Updated", updated!.Title);
        }
    }
}
