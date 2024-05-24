using SchoolServer.Core.Enums;
using SchoolServer.Core.Models;

namespace SchoolServer.Application.Interfaces.Repositories
{
    public interface IUsersRepository
    {
        Task Add(User user);
        Task<List<User>> Get();
        Task<User> GetByUsername(string username);
        public Task AddUserRole(string username, Role role);
        Task<HashSet<Permission>> GetUserPermissions(string username);
        Task DeleteByUsername(string username);
        Task Update(User user);
    }
}