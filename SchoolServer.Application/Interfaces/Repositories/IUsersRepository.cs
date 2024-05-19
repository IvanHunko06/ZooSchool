using SchoolServer.Core.Enums;
using SchoolServer.Core.Models;

namespace SchoolServer.Application.Interfaces.Repositories
{
    public interface IUsersRepository
    {
        Task Add(User user);
        Task<List<User>> Get();
        Task<User> GetByUsername(string username);
        Task<User> GetByUserId(Guid userId);
        public Task AddUserRole(Guid userId, Role role);
        Task<HashSet<Permission>> GetUserPermissions(Guid userId);
    }
}