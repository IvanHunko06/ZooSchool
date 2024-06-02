using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolServer.Core.Models;
using SchoolServer.Application.Exceptions;
using SchoolServer.Application.Interfaces.Repositories;
namespace SchoolServer.DataAccess.SQLServer.Repositories;

public class TestsRepository :ITestsRepository
{
    private readonly SchoolServerDbContext context;

    public TestsRepository(SchoolServerDbContext context)
    {
        this.context = context;
    }
    public async Task AddTest(Test test)
    {
        try
        {
            await context.Tests.AddAsync(new Models.TestEntity()
            {
                Title = test.Title,
                ContentUrl = test.ContentUrl,
                AnswersFile = test.AnswersFile
            });
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
        {
            throw new TestAlreadyExistException();
        }
        catch
        {
            throw;
        }

    }

    public async Task<Test> GetTestById(int id)
    {
        var testsM = await context.
            Tests
            .AsNoTracking()
            .FirstOrDefaultAsync(x=>x.Id == id) ?? throw new TestNotFoundException();

        var result = new Test()
        {
            Id = testsM.Id,
            ContentUrl = testsM.ContentUrl,
            AnswersFile = testsM.AnswersFile,
            Title = testsM.Title,
        };
        return result;
    }

    public async Task<List<Test>> GetAllTests()
    {
        var testsM = await context.Tests.AsNoTracking().ToListAsync();
        var result = new List<Test>();
        foreach(var test in testsM)
        {
            result.Add(new Test()
            {
                Id = test.Id,
                AnswersFile = test.AnswersFile,
                Title = test.Title,
                ContentUrl = test.ContentUrl,
            });
        }
        return result;
    }

    public async Task<int> AddAttempt(int userId, int testId)
    {
        try
        {
            var testAttemptEntity = new Models.TestAttemptEntity()
            {
                AttemptTime = DateTime.Now,
                UserId = userId,
                TestId = testId,
            };
            await context.TestAttempts.AddAsync(testAttemptEntity);
            await context.SaveChangesAsync();
            return testAttemptEntity.Id;
        }
        catch
        {
            throw;
        }
    }
    public async Task AddAnswer(int attemptId, int questionId, string answerValue)
    {
        await context.TestsAnswers.AddAsync(new Models.TestAnswerEntity()
        {
            AnswerValue = answerValue,
            QuestionId = questionId,
            AttemptId = attemptId,
        });
        await context.SaveChangesAsync();
    }

    public async Task DeleteAttempt(int id)
    {
        await context.TestAttempts.Where(a => a.Id == id).ExecuteDeleteAsync();
    }
    public async Task DeleteTest(int id)
    {
        await context.Tests.Where(a => a.Id == id).ExecuteDeleteAsync();
    }
}
