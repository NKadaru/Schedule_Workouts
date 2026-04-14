using System.Text.Json.Serialization;

namespace WorkoutScheduler.Api.Models;

public class WhoopTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = "";
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = "";
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}

public class WhoopRecovery
{
    [JsonPropertyName("cycle_id")]
    public long CycleId { get; set; }
    [JsonPropertyName("score_state")]
    public string ScoreState { get; set; } = "";
    [JsonPropertyName("score")]
    public WhoopRecoveryScore? Score { get; set; }
}

public class WhoopRecoveryScore
{
    [JsonPropertyName("recovery_score")]
    public double RecoveryScore { get; set; }
    [JsonPropertyName("resting_heart_rate")]
    public double RestingHeartRate { get; set; }
    [JsonPropertyName("hrv_rmssd_milli")]
    public double HrvRmssd { get; set; }
    [JsonPropertyName("spo2_percentage")]
    public double? Spo2 { get; set; }
    [JsonPropertyName("skin_temp_celsius")]
    public double? SkinTemp { get; set; }
}

public class WhoopCycle
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    [JsonPropertyName("start")]
    public string Start { get; set; } = "";
    [JsonPropertyName("end")]
    public string? End { get; set; }
    [JsonPropertyName("score")]
    public WhoopCycleScore? Score { get; set; }
}

public class WhoopCycleScore
{
    [JsonPropertyName("strain")]
    public double Strain { get; set; }
    [JsonPropertyName("kilojoule")]
    public double Kilojoule { get; set; }
    [JsonPropertyName("average_heart_rate")]
    public int AvgHeartRate { get; set; }
    [JsonPropertyName("max_heart_rate")]
    public int MaxHeartRate { get; set; }
}

public class WhoopSleep
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";
    [JsonPropertyName("cycle_id")]
    public long CycleId { get; set; }
    [JsonPropertyName("start")]
    public string Start { get; set; } = "";
    [JsonPropertyName("end")]
    public string End { get; set; } = "";
    [JsonPropertyName("nap")]
    public bool Nap { get; set; }
    [JsonPropertyName("score")]
    public WhoopSleepScore? Score { get; set; }
}

public class WhoopSleepScore
{
    [JsonPropertyName("sleep_performance_percentage")]
    public double? SleepPerformance { get; set; }
    [JsonPropertyName("sleep_efficiency_percentage")]
    public double? SleepEfficiency { get; set; }
    [JsonPropertyName("respiratory_rate")]
    public double? RespiratoryRate { get; set; }
    [JsonPropertyName("stage_summary")]
    public WhoopSleepStages? Stages { get; set; }
}

public class WhoopSleepStages
{
    [JsonPropertyName("total_in_bed_time_milli")]
    public long TotalInBed { get; set; }
    [JsonPropertyName("total_light_sleep_time_milli")]
    public long LightSleep { get; set; }
    [JsonPropertyName("total_slow_wave_sleep_time_milli")]
    public long DeepSleep { get; set; }
    [JsonPropertyName("total_rem_sleep_time_milli")]
    public long RemSleep { get; set; }
    [JsonPropertyName("total_awake_time_milli")]
    public long AwakeTime { get; set; }
}

public class WhoopPaginatedResponse<T>
{
    [JsonPropertyName("records")]
    public List<T> Records { get; set; } = new();
    [JsonPropertyName("next_token")]
    public string? NextToken { get; set; }
}

// Frontend-friendly summary
public class WhoopDashboard
{
    public double? RecoveryScore { get; set; }
    public double? RestingHeartRate { get; set; }
    public double? Hrv { get; set; }
    public double? Strain { get; set; }
    public double? SleepPerformance { get; set; }
    public double? SleepHours { get; set; }
    public string RecoveryLevel { get; set; } = "unknown";
    public List<DailyEntry> MonthlyRecovery { get; set; } = new();
    public List<DailyEntry> MonthlyStrain { get; set; } = new();
    public List<DailyEntry> MonthlySleep { get; set; } = new();
    public List<DailySummary> DailyHistory { get; set; } = new();
}

public class DailyEntry
{
    public string Date { get; set; } = "";
    public double Value { get; set; }
    public string Level { get; set; } = ""; // green/yellow/red for recovery
}

public class DailySummary
{
    public string Date { get; set; } = "";
    public double? Recovery { get; set; }
    public string RecoveryLevel { get; set; } = "";
    public double? Strain { get; set; }
    public double? SleepHours { get; set; }
    [JsonIgnore]
    public string SortKey { get; set; } = "";
}
