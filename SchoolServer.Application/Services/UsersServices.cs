using Microsoft.Extensions.Logging;
using SchoolServer.Application.Exceptions;
using SchoolServer.Application.Interfaces.Auth;
using SchoolServer.Application.Interfaces.Repositories;
using SchoolServer.Core.Models;
namespace SchoolServer.Application.Services;

public class UsersServices
{
    private readonly IPasswordHasher passwordHasher;
    private readonly IUsersRepository usersRepository;
    private readonly IJWTProvider jWTProvider;
    private readonly ILogger<UsersServices> logger;

    public UsersServices(IPasswordHasher passwordHasher, ILogger<UsersServices> logger, IUsersRepository usersRepository, IJWTProvider jWTProvider)
    {
        this.passwordHasher = passwordHasher;
        this.logger = logger;
        this.usersRepository = usersRepository;
        this.jWTProvider = jWTProvider;
    }

    public async Task Register(string username, string password)
    {
        var hashedPassword = passwordHasher.Generate(password);

        var user = User.Create(username, hashedPassword);
        try
        {
            await usersRepository.Add(user);
            await usersRepository.AddUserRole(user.Username, Core.Enums.Role.User);
            logger.LogInformation("User successfully added to database");
        }
        catch(UserAlreadyExistException)
        {
            logger.LogError("User already exist in database");
            throw;
        }
        catch
        {
            throw;
        }

        
    }
    public async Task ChangePassword(string username, string currentPassword, string newPassword)
    {
        try
        {
            var user = await GetByUsername(username);
            if (!passwordHasher.Verify(currentPassword, user.PasswordHash)) throw new IncorrectPasswordException();      
            string newPasswordHash = passwordHasher.Generate(newPassword);
            User newUser = User.Create(username, newPasswordHash);
            await usersRepository.Update(newUser);

        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            throw;
        }
    }
    public async Task ChangePassword(string username, string newPassword)
    {
        try
        {
            var user = await GetByUsername(username);
            string newPasswordHash = passwordHasher.Generate(newPassword);
            User newUser = User.Create(user.Username, newPasswordHash);
            await usersRepository.Update(newUser);

        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            throw;
        }
    }

    public async Task<string> Login(string username, string password)
    {
        User user;
        try
        {
            user = await usersRepository.GetByUsername(username);
        }
        catch (UserNotFoundException)
        {
            logger.LogError($"User {username} not found in database");
            throw;
        }
        catch
        {
            throw;
        }
        var passwordCheck = passwordHasher.Verify(password, user.PasswordHash);
        if (!passwordCheck) { logger.LogError($"{password} not equals {user.PasswordHash}. Wrong password");  throw new IncorrectPasswordException(); }

        var token = jWTProvider.GenerateToken(user);
        return token;
    }

    public async Task<User> GetByUsername(string username)
    {
        try
        {
            var user = await usersRepository.GetByUsername(username);
            return user;
        }
        catch (UserNotFoundException)
        {
            logger.LogError($"Username {username} not found in database");
            throw;
        }
        catch
        {
            throw;
        }
    }

    public async Task DeleteAccount(string targetUsername)
    {
        try
        {
            await usersRepository.DeleteByUsername(targetUsername);
            logger.LogInformation("Account deleated");
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            throw;
        }
    }


    public async Task AddFavouriteLesson(string username, int lessonId)
    {

    }
}