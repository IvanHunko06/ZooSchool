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

        var user = User.Create(Guid.NewGuid(), username, hashedPassword);
        try
        {
            await usersRepository.Add(user);
            await usersRepository.AddUserRole(user.Id, Core.Enums.Role.User);
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
        if (!passwordCheck) { logger.LogError($"{password} not equals {user.PasswordHash}. Wrong password");  throw new IncorrectLoginPasswordException(); }

        if (user.IsBanned)
            throw new UserBannedException();
        var token = jWTProvider.GenerateToken(user);
        return token;
    }
    public async Task<User> GetUserByJWTToken(string jwtToken)
    {
        Guid userId = jWTProvider.GetUserIdFromToken(jwtToken);
        try
        {
            var user = await usersRepository.GetByUserId(userId);
            return user;
        }catch(UserNotFoundException)
        {
            logger.LogError($"UserId {userId.ToString()} not found in database");
            throw;
        }
        catch
        {
            throw;
        }

    }
}