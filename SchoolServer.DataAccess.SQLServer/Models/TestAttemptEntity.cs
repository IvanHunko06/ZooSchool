namespace SchoolServer.DataAccess.SQLServer.Models;

public class TestAttemptEntity
{
    public int Id { get; set; }
    public UserEntity User { get; set; }
    public TestEntity Test {  get; set; }
    public int UserId {  get; set; }
    public int TestId {  get; set; }
    public DateTime AttemptTime { get; set; }
    public List<TestAnswerEntity> Answers { get; set; }
}
