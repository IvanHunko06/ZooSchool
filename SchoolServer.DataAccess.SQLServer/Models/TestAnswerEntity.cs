namespace SchoolServer.DataAccess.SQLServer.Models;

public class TestAnswerEntity
{
    public int Id { get; set; }
    public int AttemptId {  get; set; }
    public int QuestionId {  get; set; }
    public string AnswerValue {  get; set; } = string.Empty;
    public TestAttemptEntity Attempt { get; set; }
}
