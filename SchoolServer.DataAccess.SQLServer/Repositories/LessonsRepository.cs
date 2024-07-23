using Microsoft.EntityFrameworkCore;
using SchoolServer.Core.Models;
using SchoolServer.Application.Exceptions;
using SchoolServer.Application.Interfaces.Repositories;
using SchoolServer.DataAccess.SQLServer.Models;
namespace SchoolServer.DataAccess.SQLServer.Repositories
{
    public class LessonsRepository : ILessonsRepository
    {
        private readonly SchoolServerDbContext context;

        public LessonsRepository(SchoolServerDbContext context)
        {
            this.context = context;
        }
        public async Task<List<Lesson>> Get()
        {
            var list = new List<Lesson>();
            var lessonModel = await context.Lessons
                .AsNoTracking()
                .ToListAsync();
            foreach (var lesson in lessonModel)
            {
                list.Add(new Lesson()
                {
                    Id = lesson.Id,
                    Title = lesson.Title,
                    ImageUrl = lesson.TitleImageUrl,
                    ContentUrl = lesson.ContentUrl,
                });
            }
            return list;
        }
        public async Task<Lesson> Get(int id)
        {
            var lessonModel = await context.Lessons
                .AsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync() ?? throw new LessonNotFoundException();
            var lesson = new Lesson()
            {
                Id = lessonModel.Id,
                Title = lessonModel.Title,
                ImageUrl = lessonModel.TitleImageUrl,
                ContentUrl = lessonModel.ContentUrl,
            };
            return lesson;
        }

        public async Task Add(Lesson lesson)
        {
            LessonEntity entity = new LessonEntity()
            {
                Title = lesson.Title,
                TitleImageUrl = lesson.ImageUrl,
                ContentUrl = lesson.ContentUrl,
                LastUpdate = null
            };
            try
            {
                await context.Lessons.AddAsync(entity);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }

        }

        public async Task Update(Lesson lesson)
        {
            await context.Lessons
                .Where(l => l.Id == lesson.Id)
                .ExecuteUpdateAsync(s => s
                .SetProperty(l => l.Title, lesson.Title)
                .SetProperty(l=>l.LastUpdate, DateTime.Now)
                .SetProperty(l=>l.ContentUrl, lesson.ContentUrl)
                .SetProperty(l=>l.TitleImageUrl, lesson.ImageUrl));
        }
        public async Task Delete(int id)
        {
            await context.Lessons.Where(l=>l.Id == id)
                .ExecuteDeleteAsync(); 
        }

    }
}
