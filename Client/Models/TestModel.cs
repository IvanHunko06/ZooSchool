using System.Text.Json;
using System.Text.Json.Serialization;

namespace Client.Models;

public class TestModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("contentUrl")]
    public string ContentUrl { get; set; }
}
public class UserAnswer
{
    [JsonPropertyName("questionId")]
    public int QuestionId { get; set; }
    [JsonPropertyName("answerValue")]
    public string AnswerValue { get; set; } = string.Empty;
}