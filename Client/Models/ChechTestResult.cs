using System.Text.Json.Serialization;

namespace Client.Models;

public class ChechTestResult
{
    [JsonPropertyName("pointsScored")]
    public int PointsScored { get; set; }

    [JsonPropertyName("totalPoints")]
    public int TotalPoints { get; set; }
}
