using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolServer.Application.Exceptions;
using SchoolServer.Application.Interfaces.Repositories;
using SchoolServer.Core.Enums;
using SchoolServer.Core.Models;
using SchoolServer.DataAccess.SQLServer.Models;
using System.Diagnostics;
namespace SchoolServer.DataAccess.SQLServer.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly SchoolServerDbContext context;

    public UsersRepository(SchoolServerDbContext context)
    {
        this.context = context;
    }


    public async Task AddUserRole(Guid userId, Role role)
    {

        var userEntity = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == userId) ?? throw new UserNotFoundException();
        var roleEntity = await context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == (int)role) ?? throw new RoleNotFoundException();

        try
        {
            var userRole = new UserRoleEntity()
            {
                RoleId = roleEntity.Id,
                UserId = userEntity.Id
            };
            context.UsersRoles.Add(userRole);
            await context.SaveChangesAsync();
        }
        catch
        {
            throw;
        }

    }
    public async Task<List<User>> Get()
    {
        var userEntitis = await context.Users
            .AsNoTracking()
            .ToListAsync();
        List<User> users = new List<User>();
        foreach (var user in userEntitis)
        {
            users.Add(User.Create(user.UserId, user.Username, user.Password, user.BannedByUser != null ? true : false));
        }
        return users;
    }

    /// <summary>
    /// Adds user to database
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// <exception cref="UserAlreadyExistException"></exception>
    public async Task Add(User user)
    {
        var userEntity = new UserEntity()
        {
            UserId = user.Id,
            Username = user.UserName,
            Password = user.PasswordHash,
            BannedByUser = null
        };
        try
        {
            await context.Users.AddAsync(userEntity);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
        {
            Debug.WriteLine(sqlEx.Number);
            throw new UserAlreadyExistException();
        }
        catch
        {
            throw;
        }

    }

    /// <summary>
    /// Returns user object by username or throws an exception
    /// </summary>
    /// <param name="username"></param>
    /// <returns>User object</returns>
    /// <exception cref="UserNotFoundException"></exception>
    public async Task<User> GetByUsername(string username)
    {
        var userEntity = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username) ?? throw new UserNotFoundException();
        return User.Create(userEntity.UserId, userEntity.Username, userEntity.Password, userEntity.BannedByUser != null ? true : false);
    }
    public async Task<User> GetByUserId(Guid userId)
    {
        var userEntity = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == userId) ?? throw new UserNotFoundException();
        return User.Create(userEntity.UserId, userEntity.Username, userEntity.Password, userEntity.BannedByUser != null ? true : false);
    }

    public async Task<HashSet<Permission>> GetUserPermissions(Guid userId)
    {
        var roles = await context.Users.AsNoTracking()
            .Include(u => u.Roles)
            .ThenInclude(u => u.Permissions)
            .Where(u => u.UserId == userId)
            .Select(u => u.Roles)
            .ToArrayAsync();
        return roles.SelectMany(r => r)
            .SelectMany(r => r.Permissions)
            .Select(p => (Permission)p.Id)
            .ToHashSet();
    }
}
