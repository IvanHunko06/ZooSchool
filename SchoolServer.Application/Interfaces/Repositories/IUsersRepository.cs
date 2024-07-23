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
        Task<List<int>> GetFavouriteLessonsId(string username);
        Task AddUserFavouriteLesson(string username, int lessonId);
        Task RemoveUserFavouriteLesson(string username, int lessonId);
    }
}