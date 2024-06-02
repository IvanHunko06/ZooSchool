using System.Text.Json.Serialization;

namespace Client.Models;

public class PageElementModel
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("value")]
    public string Value { get; set; }
    [JsonPropertyName("styles")]
    public Dictionary<string,string> Styles { get; set; }
}

