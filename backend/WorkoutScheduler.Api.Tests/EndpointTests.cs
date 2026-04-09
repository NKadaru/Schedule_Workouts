using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace WorkoutScheduler.Api.Tests;

public class EndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public EndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsOkWithExpectedJson()
    {
        // Validates: Requirements 1.5
        var response = await _client.GetAsync("/api/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal("healthy", json.GetProperty("status").GetString());
        Assert.True(json.TryGetProperty("timestamp", out var timestamp));
        Assert.False(string.IsNullOrWhiteSpace(timestamp.GetString()));
    }

    [Fact]
    public async Task WeatherForecastEndpoint_ReturnsOkWithJsonArray()
    {
        // Validates: Requirements 4.2, 4.3
        var response = await _client.GetAsync("/api/weatherforecast");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(JsonValueKind.Array, json.ValueKind);
        Assert.True(json.GetArrayLength() > 0, "Expected at least one forecast element");

        foreach (var element in json.EnumerateArray())
        {
            Assert.True(element.TryGetProperty("date", out _), "Missing 'date' property");
            Assert.True(element.TryGetProperty("temperatureC", out _), "Missing 'temperatureC' property");
            Assert.True(element.TryGetProperty("temperatureF", out _), "Missing 'temperatureF' property");
            Assert.True(element.TryGetProperty("summary", out _), "Missing 'summary' property");
        }
    }
}
