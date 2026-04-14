using System.Net.Http.Headers;
using System.Text.Json;
using WorkoutScheduler.Api.Models;

namespace WorkoutScheduler.Api.Services;

public class WhoopService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<WhoopService> _logger;
    private readonly string _tokenFilePath;
    private const string BaseUrl = "https://api.prod.whoop.com";

    private static string? _accessToken;
    private static string? _refreshToken;
    private static DateTime _tokenExpiry = DateTime.MinValue;

    public WhoopService(HttpClient httpClient, IConfiguration config, ILogger<WhoopService> logger, IWebHostEnvironment env)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
        _tokenFilePath = Path.Combine(env.ContentRootPath, "Data", "whoop-token.json");
        LoadTokenFromFile();
    }

    public bool IsConnected => !string.IsNullOrEmpty(_accessToken);

    private void LoadTokenFromFile()
    {
        if (_accessToken != null) return; // already loaded
        try
        {
            if (File.Exists(_tokenFilePath))
            {
                var json = File.ReadAllText(_tokenFilePath);
                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (data != null)
                {
                    _accessToken = data.GetValueOrDefault("access_token");
                    _refreshToken = data.GetValueOrDefault("refresh_token");
                    if (data.TryGetValue("expiry", out var exp) && DateTime.TryParse(exp, out var dt))
                        _tokenExpiry = dt;
                    _logger.LogInformation("Loaded WHOOP token from file");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not load WHOOP token file");
        }
    }

    private void SaveTokenToFile()
    {
        try
        {
            var data = new Dictionary<string, string>
            {
                ["access_token"] = _accessToken ?? "",
                ["refresh_token"] = _refreshToken ?? "",
                ["expiry"] = _tokenExpiry.ToString("O")
            };
            File.WriteAllText(_tokenFilePath, JsonSerializer.Serialize(data));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not save WHOOP token file");
        }
    }

    public string GetAuthorizeUrl()
    {
        var clientId = _config["Whoop:ClientId"];
        var redirectUri = _config["Whoop:RedirectUri"];
        var scopes = "read:recovery read:cycles read:sleep read:workout read:profile";
        return $"{BaseUrl}/oauth/oauth2/auth?client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri!)}&response_type=code&scope={Uri.EscapeDataString(scopes)}&state=grindflow";
    }

    public async Task<bool> ExchangeCodeAsync(string code)
    {
        var tokenUrl = $"{BaseUrl}/oauth/oauth2/token";
        var body = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["client_id"] = _config["Whoop:ClientId"]!,
            ["client_secret"] = _config["Whoop:ClientSecret"]!,
            ["redirect_uri"] = _config["Whoop:RedirectUri"]!
        });

        try
        {
            var response = await _httpClient.PostAsync(tokenUrl, body);
            var json = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Token exchange response: {Status}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Token exchange failed: {Body}", json);
                return false;
            }

            var token = JsonSerializer.Deserialize<WhoopTokenResponse>(json);
            if (token == null) return false;

            _accessToken = token.AccessToken;
            _refreshToken = token.RefreshToken;
            _tokenExpiry = DateTime.UtcNow.AddSeconds(token.ExpiresIn - 60);
            SaveTokenToFile();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token exchange error");
            return false;
        }
    }

    public async Task<WhoopDashboard> GetDashboardAsync()
    {
        await EnsureTokenAsync();
        var dashboard = new WhoopDashboard();
        var oneMonthAgo = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        try
        {
            // Fetch all three data sets for the last 30 days
            var allCycles = await GetAllPagesAsync<WhoopCycle>(
                $"/developer/v2/cycle?limit=25&start={oneMonthAgo}");
            var allRecoveries = await GetAllPagesAsync<WhoopRecovery>(
                $"/developer/v2/recovery?limit=25&start={oneMonthAgo}");
            var allSleeps = await GetAllPagesAsync<WhoopSleep>(
                $"/developer/v2/activity/sleep?limit=25&start={oneMonthAgo}");

            _logger.LogInformation("Fetched {Cycles} cycles, {Recoveries} recoveries, {Sleeps} sleeps",
                allCycles.Count, allRecoveries.Count, allSleeps.Count);

            // Index recovery by cycle_id
            var recoveryByCycleId = allRecoveries
                .Where(r => r.Score != null && r.ScoreState == "SCORED")
                .ToDictionary(r => r.CycleId, r => r);

            // Index sleep by cycle_id (exclude naps)
            var sleepByCycleId = allSleeps
                .Where(s => s.Score?.Stages != null && !s.Nap)
                .GroupBy(s => s.CycleId)
                .ToDictionary(g => g.Key, g => g.First());

            // Set today's values from latest
            if (allRecoveries.Count > 0 && allRecoveries[0].Score != null)
            {
                var latest = allRecoveries[0].Score!;
                dashboard.RecoveryScore = latest.RecoveryScore;
                dashboard.RestingHeartRate = latest.RestingHeartRate;
                dashboard.Hrv = Math.Round(latest.HrvRmssd, 1);
                dashboard.RecoveryLevel = latest.RecoveryScore switch
                {
                    >= 67 => "green", >= 34 => "yellow", _ => "red"
                };
            }

            if (allCycles.Count > 0 && allCycles[0].Score != null)
                dashboard.Strain = Math.Round(allCycles[0].Score!.Strain, 1);

            var latestSleep = allSleeps.FirstOrDefault(s => !s.Nap && s.Score != null);
            if (latestSleep?.Score != null)
            {
                dashboard.SleepPerformance = latestSleep.Score!.SleepPerformance;
                if (latestSleep.Score!.Stages != null)
                    dashboard.SleepHours = Math.Round(latestSleep.Score!.Stages!.TotalInBed / 3600000.0, 1);
            }

            // Build daily history from cycles (the anchor)
            foreach (var cycle in allCycles.Where(c => c.Score != null))
            {
                var rawDate = cycle.Start.Length >= 10 ? cycle.Start[..10] : cycle.Start;
                var displayDate = DateTime.TryParse(rawDate, out var dt) ? dt.ToString("MMM-dd") : rawDate;
                var summary = new DailySummary
                {
                    Date = displayDate,
                    Strain = Math.Round(cycle.Score!.Strain, 1),
                    SortKey = rawDate
                };

                // Match recovery by cycle ID
                if (recoveryByCycleId.TryGetValue(cycle.Id, out var rec))
                {
                    summary.Recovery = Math.Round(rec.Score!.RecoveryScore, 0);
                    summary.RecoveryLevel = rec.Score.RecoveryScore switch
                    {
                        >= 67 => "green", >= 34 => "yellow", _ => "red"
                    };
                }

                // Match sleep by cycle ID (exclude naps)
                if (sleepByCycleId.TryGetValue(cycle.Id, out var slp))
                {
                    summary.SleepHours = Math.Round(slp.Score!.Stages!.TotalInBed / 3600000.0, 1);
                }

                dashboard.DailyHistory.Add(summary);
            }

            // Sort newest first
            dashboard.DailyHistory = dashboard.DailyHistory
                .OrderByDescending(d => d.SortKey)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching WHOOP dashboard data");
        }

        return dashboard;
    }

    public async Task<object> GetDebugDataAsync()
    {
        await EnsureTokenAsync();
        var oneMonthAgo = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        var cycles = await GetAllPagesAsync<WhoopCycle>(
            $"/developer/v2/cycle?limit=3&start={oneMonthAgo}");
        var recoveries = await GetAllPagesAsync<WhoopRecovery>(
            $"/developer/v2/recovery?limit=3&start={oneMonthAgo}");
        var sleeps = await GetAllPagesAsync<WhoopSleep>(
            $"/developer/v2/activity/sleep?limit=3&start={oneMonthAgo}");

        return new
        {
            cyclesSample = cycles.Take(3).Select(c => new { c.Id, c.Start, c.End, Strain = c.Score?.Strain }),
            recoveriesSample = recoveries.Take(3).Select(r => new { r.CycleId, r.ScoreState, Recovery = r.Score?.RecoveryScore, Hrv = r.Score?.HrvRmssd }),
            sleepsSample = sleeps.Take(3).Select(s => new { s.Id, s.Start, s.End, SleepPerf = s.Score?.SleepPerformance, InBed = s.Score?.Stages?.TotalInBed }),
            cycleIds = cycles.Take(5).Select(c => c.Id),
            recoveryCycleIds = recoveries.Take(5).Select(r => r.CycleId)
        };
    }

    private async Task<List<T>> GetAllPagesAsync<T>(string initialPath)
    {
        var all = new List<T>();
        var path = initialPath;

        for (var page = 0; page < 5; page++) // max 5 pages = ~125 records
        {
            var result = await GetAsync<WhoopPaginatedResponse<T>>(path);
            if (result == null || result.Records.Count == 0) break;

            all.AddRange(result.Records);

            if (string.IsNullOrEmpty(result.NextToken)) break;

            // Append or replace nextToken in the path
            path = initialPath.Contains('?')
                ? $"{initialPath}&nextToken={result.NextToken}"
                : $"{initialPath}?nextToken={result.NextToken}";
        }

        return all;
    }

    private async Task<T?> GetAsync<T>(string path)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}{path}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        var response = await _httpClient.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("WHOOP API {Path}: {Status} - {Body}", path, response.StatusCode, json);
            return default;
        }

        _logger.LogInformation("WHOOP API {Path}: {Length} bytes", path, json.Length);

        try
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize WHOOP response for {Path}: {Json}", path, json[..Math.Min(500, json.Length)]);
            return default;
        }
    }

    private async Task EnsureTokenAsync()
    {
        if (DateTime.UtcNow < _tokenExpiry || string.IsNullOrEmpty(_refreshToken)) return;

        var tokenUrl = $"{BaseUrl}/oauth/oauth2/token";
        var body = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = _refreshToken,
            ["client_id"] = _config["Whoop:ClientId"]!,
            ["client_secret"] = _config["Whoop:ClientSecret"]!
        });

        var response = await _httpClient.PostAsync(tokenUrl, body);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<WhoopTokenResponse>(json);
            if (token != null)
            {
                _accessToken = token.AccessToken;
                _refreshToken = token.RefreshToken;
                _tokenExpiry = DateTime.UtcNow.AddSeconds(token.ExpiresIn - 60);
                SaveTokenToFile();
            }
        }
    }
}
