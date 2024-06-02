using SchoolServer.Core.Models;

namespace SchoolServer.Application.Interfaces.Repositories;

public interface ITestsRepository
{
    Task AddTest(Test test);
    Task<Test> GetTestById(int id);
    Task<int> AddAttempt(int userId, int testId);
    Task AddAnswer(int attemptId, int questionId, string answerValue);
    Task DeleteAttempt(int id);
    Task DeleteTest(int id);
    Task<List<Test>> GetAllTests();
}
