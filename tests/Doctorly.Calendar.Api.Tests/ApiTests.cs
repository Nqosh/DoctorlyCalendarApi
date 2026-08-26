using System.Net;
using System.Net.Http.Json;
using Doctorly.Calendar.Application.Events.Requests;
using Doctorly.Calendar.Application.Events.Responses;
using Doctorly.Calendar.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Doctorly.Calendar.Api.Tests;

public class ApiTests(TestFactory factory) : IClassFixture<TestFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Create_then_get()
    {
        var start = new DateTimeOffset(
            2026,
            9,
            1,
            8,
            0,
            0,
            TimeSpan.Zero);

        var request = new CreateEventRequest(
            "Check-up",
            "Routine",
            start,
            start.AddMinutes(30),
            [
                new AttendeeInput(
                    "Alex",
                    "alex@test.dev")
            ]);

        var response = await _client.PostAsJsonAsync( "/api/v1/events", request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<EventResponse>();

        Assert.NotNull(created);

        var fetched =await _client.GetAsync($"/api/v1/events/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, fetched.StatusCode);
    }

    [Fact]
    public async Task Invalid_range_returns_400()
    {
        var start = new DateTimeOffset(
            2026,
            9,
            1,
            8,
            0,
            0,
            TimeSpan.Zero);

        var request = new CreateEventRequest(
            "Bad",
            string.Empty,
            start,
            start,
            [
                new AttendeeInput(
                    "Alex",
                    "alex2@test.dev")
            ]);

        var response =
            await _client.PostAsJsonAsync("/api/v1/events", request);

        Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode);
    }
}

public class TestFactory: WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<
                DbContextOptions<CalendarDbContext>>();

            services.AddDbContext<CalendarDbContext>(
                options =>
                {
                    options.UseInMemoryDatabase(
                        $"calendar-{Guid.NewGuid()}");
                });
        });
    }
}