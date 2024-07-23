using System.Text.Json.Serialization;

namespace Client.Models;

public class PasswordChangeRequest
{
    [JsonPropertyName("currentPassword")]
    public string CurrentPassword { get; set; } = "";
    [JsonPropertyName("newPassword")]
    public string NewPassword { get; set; } = "";
}

public class PasswordChangeResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }
}