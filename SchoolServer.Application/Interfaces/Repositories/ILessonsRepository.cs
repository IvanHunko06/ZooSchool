using SchoolServer.Core.Models;

namespace SchoolServer.Application.Interfaces.Repositories
{
    public interface ILessonsRepository
    {
        Task<List<Lesson>> Get();
        Task<Lesson> Get(int id);
        Task Add(Lesson lesson);
        Task Update(Lesson lesson);
        Task Delete(int id);
    }
}