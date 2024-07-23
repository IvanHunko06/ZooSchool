using System.Text.Json;
using System.Text.Json.Serialization;

namespace Client.Models;

public class AccountModel
{
    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("favoriteLessons")]
    public List<int> FavoriteLessons { get; set; }
}
