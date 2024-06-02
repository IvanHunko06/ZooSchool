using Microsoft.Extensions.Logging;
using SchoolServer.Application.Exceptions;
using SchoolServer.Application.Interfaces.Repositories;
using SchoolServer.Core.Models;
namespace SchoolServer.Application.Services;

public class TestsService
{
    private readonly ITestsRepository testsRepository;
    private readonly ILogger<TestsService> logger;
    private readonly IUsersRepository usersRepository;

    public TestsService(ITestsRepository testsRepository, ILogger<TestsService> logger, IUsersRepository usersRepository)
    {
        this.testsRepository = testsRepository;
        this.logger = logger;
        this.usersRepository = usersRepository;
    }
    public TestSummary CheckTest(List<TestAnswer> userAnswers, List<TestQuestion> testQuestions)
    {
        TestSummary testSummary = new TestSummary();
        foreach (var testQuestion in testQuestions)
        {
            testSummary.TotalPoints += testQuestion.Score; 
            string userAnswer = (userAnswers.FirstOrDefault(t => t.QuestionId == testQuestion.QuestionId)?.AnswerText) ?? "";
            string[] answerValues = testQuestion.AnswerText.Trim().Split(' ');
            testSummary.Answers.Add(new AnswerAnalyzer()
            {
                QuestionId = testQuestion.QuestionId,
                UserAnswer = userAnswer,
                CorrectAnswer = testQuestion.AnswerText
            });
            if (answerValues.Length == 1)
            {
                if (!string.IsNullOrWhiteSpace(userAnswer) && userAnswer == testQuestion.AnswerText)
                    testSummary.PointsScored += testQuestion.Score;
            }
            else
            {
                string[] userAnswersValues = userAnswer.Trim().Split(' ');
                foreach (var userAnswersValue in userAnswersValues)
                {
                    if(answerValues.Contains(userAnswersValue))
                        testSummary.PointsScored += (int)Math.Round( (double)testQuestion.Score/(double)answerValues.Length);
                }
            }


        }
        return testSummary;
    }
    public List<int> GetCorrectAnswersId(TestSummary testSummary)
    {
        List<int> result = new List<int>();
        foreach(var answer in testSummary.Answers)
        {
            if(answer.UserAnswer == answer.CorrectAnswer)
                result.Add(answer.QuestionId);
        }
        return result;
    }
    public List<int> GetWrongAnswersId(TestSummary testSummary)
    {
        List<int> result = new List<int>();
        foreach (var answer in testSummary.Answers)
        {
            if (answer.UserAnswer != answer.CorrectAnswer)
                result.Add(answer.QuestionId);
        }
        return result;
    }
    public async Task UploadTestResult(TestSummary testSummary, string username, int testId)
    {
        try
        {
            var userId = (await usersRepository.GetByUsername(username)).Id;
            var attemptId = await testsRepository.AddAttempt(userId, testId);
            foreach (var answer in testSummary.Answers)
            {
                await testsRepository.AddAnswer(attemptId, answer.QuestionId, answer.UserAnswer);
            }
        }
        catch
        {
            throw;
        }

    }
    public async Task CreateTest(Test test)
    {
        try
        {
            await testsRepository.AddTest(test);
            logger.LogInformation("Test succesfully added");
        }
        catch (TestAlreadyExistException)
        {
            logger.LogError("Test already exist in database");
            throw;
        }
        catch
        {
            throw;
        }
    }

    public async Task<Test> GetTestById(int id)
    {
        try
        {
            return await testsRepository.GetTestById(id);
        }
        catch(TestNotFoundException)
        {
            logger.LogError($"Test id:{id} not found in database");
            throw;
        }
        catch
        {
            throw;
        }
    }

    public async Task DeleteAttempt(int id)
    {
        await testsRepository.DeleteAttempt(id);
    }
    public async Task DeleteTest(int id)
    {
        await testsRepository.DeleteTest(id);
    }

    public async Task<List<Test>> GetAllTests()
    {
        return await testsRepository.GetAllTests();
    }

}
public class TestSummary
{
    public int PointsScored { get; set; }
    public int TotalPoints { get; set; }
    public List<AnswerAnalyzer> Answers { get; } = new List<AnswerAnalyzer>();
}
public class AnswerAnalyzer
{
    public int QuestionId { get; set; }
    public string UserAnswer { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
}