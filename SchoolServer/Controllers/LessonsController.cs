using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolServer.API.Models;
using SchoolServer.Application.Services;
using SchoolServer.Core.Models;
using SchoolServer.Infrastructure.Authentification;
namespace SchoolServer.API.Controllers;

[Authorize]
[Route("api/lessons")]
public class LessonsController : Controller
{
    private readonly LessonsService lessonsService;

    public LessonsController(LessonsService lessonsService)
    {
        this.lessonsService = lessonsService;
    }
    [HttpGet("get_all")]
    public async Task<ActionResult> GetAll()
    {
        try
        {
            var lessons = await lessonsService.GetLessons();
            return Ok(lessons);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost("create_lesson")]
    [HasPermission(Core.Enums.Permission.CreateLesson)]
    public async Task<ActionResult> CreateLesson([FromBody] LessonModel lesson)
    {
        try
        {
            var lessonC = new Lesson()
            {
                Title = lesson.Title,
                ImageUrl = lesson.ImageUrl,
                ContentUrl = lesson.ContentUrl
            };
            await lessonsService.AddLesson(lessonC);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    [HttpDelete("delete_lesson/{id:int}")]
    [HasPermission(Core.Enums.Permission.DeleteLesson)]
    public async Task<ActionResult> DeleteLesson(int id)
    {
        try
        {
            await lessonsService.DeleteLesson(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    [HttpPut("update_lesson/{id:int}")]
    [HasPermission(Core.Enums.Permission.ModifyLesson)]
    public async Task<ActionResult> UpdateLesson(int id,[FromBody] LessonModel lesson)
    {
        try
        {
            var lessonM = new Lesson()
            {
                Id = id,
                Title = lesson.Title,
                ImageUrl = lesson.ImageUrl,
                ContentUrl = lesson.ContentUrl
            };
            await lessonsService.UpdateLesson(lessonM);
            return Ok();
        }
        catch(Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }


}
