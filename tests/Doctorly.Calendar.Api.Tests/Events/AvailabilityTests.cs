using Doctorly.Calendar.Application.Events.Requests;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;
using System.Net.Http.Json;

namespace Doctorly.Calendar.Api.Tests.Events
{
    public class AvailabilityTests(TestFactory factory) : IClassFixture<TestFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task Availability_returns_true_when_no_conflict()
        {
            var request = new AvailabilityRequest(
                ["alex@test.dev"],
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow.AddMinutes(30));

            var response =
                await _client.PostAsJsonAsync(
                    "/api/v1/events/availability",
                    request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
