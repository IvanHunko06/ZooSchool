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

    public async Task DeleteByUsername(string username)
    {
        try
        {
            await context.Users
                .Where(u => u.Username == username)
                .ExecuteDeleteAsync();
        }
        catch
        {
            throw;
        }

    }


    public async Task AddUserRole(string username, Role role)
    {

        var userEntity = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username) ?? throw new UserNotFoundException();
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
            users.Add(User.Create(user.Username, user.Password));
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
            Username = user.Username,
            Password = user.PasswordHash,
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
        return User.Create(userEntity.Username, userEntity.Password);
    }

    public async Task<HashSet<Permission>> GetUserPermissions(string username)
    {
        var roles = await context.Users.AsNoTracking()
            .Include(u => u.Roles)
            .ThenInclude(u => u.Permissions)
            .Where(u => u.Username == username)
            .Select(u => u.Roles)
            .ToArrayAsync();
        return roles.SelectMany(r => r)
            .SelectMany(r => r.Permissions)
            .Select(p => (Permission)p.Id)
            .ToHashSet();
    }

    public async Task Update(User user)
    {
        await context.Users
            .Where(u => u.Username == user.Username)
            .ExecuteUpdateAsync(
                s => s
                .SetProperty(c => c.Password, user.PasswordHash)
            );
    }
}
