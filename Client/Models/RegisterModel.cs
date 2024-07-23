using System.Text.Json.Serialization;
namespace Client.Models;

public class RegisterModel
{
    [JsonPropertyName("login")]
    public string Username { get; set; } = "";
    [JsonPropertyName("password")]
    public string Password { get; set; } = "";
}
public class RegisterModelResponse
{
    [JsonPropertyName("code")]
    public int Code {  get; set; }
}