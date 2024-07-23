using Microsoft.Extensions.Logging;
using SchoolServer.Application.Interfaces.Repositories;
using SchoolServer.Core.Models;
namespace SchoolServer.Application.Services;

public class LessonsService
{
    private readonly ILogger<LessonsService> logger;
    private readonly ILessonsRepository lessonsRepository;

    public LessonsService(ILogger<LessonsService> logger, ILessonsRepository lessonsRepository)
    {
        this.logger = logger;
        this.lessonsRepository = lessonsRepository;
    }

    public async Task<List<Lesson>> GetLessons()
    {
        try
        {
            return await lessonsRepository.Get();
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            throw;
        }

    }
    public async Task AddLesson(Lesson lesson)
    {
        try
        {
            await lessonsRepository.Add(lesson);
        }catch(Exception ex)
        {
            logger.LogError(ex.Message);
            throw;
        }
    }
    public async Task DeleteLesson(int id)
    {
        try
        {
            await lessonsRepository.Delete(id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            throw;
        }
    }
    public async Task UpdateLesson(Lesson lesson)
    {
        try
        {
            await lessonsRepository.Update(lesson);
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            throw;
        }
    }
}
