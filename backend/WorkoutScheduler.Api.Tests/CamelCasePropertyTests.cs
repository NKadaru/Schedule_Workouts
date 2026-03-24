// Feature: dotnet-angular-project, Property 1: CamelCase JSON serialization round trip
using System.Text.Json;
using FsCheck;
using FsCheck.Xunit;
using WorkoutScheduler.Api.Models;

namespace WorkoutScheduler.Api.Tests;

/// <summary>
/// Property-based tests verifying that all C# model objects serialize
/// with camelCase JSON keys when using the API's configured JsonSerializerOptions.
/// **Validates: Requirements 4.3**
/// </summary>
public class CamelCasePropertyTests
{
    private static readonly JsonSerializerOptions ApiJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Converts a PascalCase property name to its expected camelCase equivalent.
    /// </summary>
    private static string ToCamelCase(string pascalCase)
    {
        if (string.IsNullOrEmpty(pascalCase))
            return pascalCase;

        return char.ToLowerInvariant(pascalCase[0]) + pascalCase[1..];
    }

    /// <summary>
    /// Asserts that every JSON key in the serialized output is the camelCase
    /// version of the corresponding C# public property name.
    /// </summary>
    private static void AssertAllKeysAreCamelCase<T>(T instance, string[] pascalPropertyNames)
    {
        var json = JsonSerializer.Serialize(instance, ApiJsonOptions);
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var expectedKeys = pascalPropertyNames
            .Select(ToCamelCase)
            .ToHashSet();

        var actualKeys = root.EnumerateObject()
            .Select(p => p.Name)
            .ToHashSet();

        Assert.Equal(expectedKeys.Count, actualKeys.Count);
        foreach (var expected in expectedKeys)
        {
            Assert.Contains(expected, actualKeys);
        }
    }

    [Property(MaxTest = 100)]
    public void WeatherForecast_AllJsonKeys_AreCamelCase(int temperatureC, string? summary)
    {
        // **Validates: Requirements 4.3**
        var forecast = new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Today),
            TemperatureC = temperatureC,
            Summary = summary
        };

        var pascalNames = new[] { "Date", "TemperatureC", "Summary", "TemperatureF" };
        AssertAllKeysAreCamelCase(forecast, pascalNames);
    }

    [Property(MaxTest = 100)]
    public void HealthResponse_AllJsonKeys_AreCamelCase(string status)
    {
        // **Validates: Requirements 4.3**
        var response = new HealthResponse
        {
            Status = status ?? "healthy",
            Timestamp = DateTime.UtcNow
        };

        var pascalNames = new[] { "Status", "Timestamp" };
        AssertAllKeysAreCamelCase(response, pascalNames);
    }
}
