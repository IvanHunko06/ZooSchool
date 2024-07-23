using Microsoft.Extensions.Logging;
using SchoolServer.Application.Exceptions;
using SchoolServer.Application.Interfaces.Repositories;

namespace SchoolServer.Application.Services;

public class FavouriteService
{

    private readonly ILogger<FavouriteService> _logger;
    private readonly IUsersRepository usersRepository;

    public FavouriteService(ILogger<FavouriteService> logger, IUsersRepository usersRepository)
    {
        _logger = logger;
        this.usersRepository = usersRepository;
    }

    public async Task AddUserFavourite(string username, int lessonId)
    {
        try
        {
            await usersRepository.AddUserFavouriteLesson(username, lessonId);
        }
        catch (UserNotFoundException)
        {
            _logger.LogError($"{username} not found in database");
            throw;
        }
        catch (LessonNotFoundException)
        {
            _logger.LogError($"Lesson id {lessonId} not found in database");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }
    public async Task RemoveUserFavourite(string username, int lessonId)
    {
        await usersRepository.RemoveUserFavouriteLesson(username, lessonId);
    }
}
