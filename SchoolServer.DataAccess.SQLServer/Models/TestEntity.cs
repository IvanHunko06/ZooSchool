namespace SchoolServer.DataAccess.SQLServer.Models;

public class TestEntity
{
    public int Id { get; set; }
    public string Title {  get; set; } = string.Empty;
    public string ContentUrl { get; set; } = string.Empty;
    public string AnswersFile { get; set; } = string.Empty;
    public List<TestAttemptEntity> Atempts { get; set; }
}
