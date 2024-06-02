using System.Text.Json.Serialization;
namespace Client.Models;

class QuestionModel
{
    [JsonPropertyName("id")] 
    public int Id { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";
    [JsonPropertyName("label")]
    public string Label { get; set; } = "";
    [JsonPropertyName("options")]
    public List<QuestionOption>? Options { get; set; } = null;
    [JsonPropertyName("pairs")]
    public List<QuestionPair>? Pairs { get; set; } = null;
    [JsonPropertyName("image")]
    public string Image { get; set; } = "";

    [JsonPropertyName("styles")]
    public Dictionary<string,string> Styles { get; set; }
}
public class QuestionOption
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";
    [JsonPropertyName("text")]
    public string Text { get; set; } = "";
}

public class QuestionPair
{
    [JsonPropertyName("left")]
    public string Left { get; set; } = "";
    [JsonPropertyName("right")]
    public string Right { get; set; } = "";
    [JsonPropertyName("leftId")]
    public string LeftId { get; set; } = "";
    [JsonPropertyName("rightId")]
    public string RightId { get; set; } = "";
}