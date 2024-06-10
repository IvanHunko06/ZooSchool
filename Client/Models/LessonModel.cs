using System.Text.Json;
using System.Text.Json.Serialization;

namespace Client.Models;
public class LessonModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("contentUrl")]
    public string ContentUrl { get; set; } = string.Empty;

    [JsonIgnore]
    public bool IsFavourite { get; set; }

}