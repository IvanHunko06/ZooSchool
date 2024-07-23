namespace SchoolServer.Core.Models;

public class TestQuestion
{
    public int QuestionId { get; set; }
    public string AnswerText { get; set; } = string.Empty;
    public int Score { get; set; }
}
