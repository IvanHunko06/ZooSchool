using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SchoolServer.API.Models;
using SchoolServer.Application.Exceptions;
using SchoolServer.Application.Interfaces.Auth;
using SchoolServer.Application.Services;
using SchoolServer.Core.Models;
using SchoolServer.Infrastructure.Authentification;
namespace SchoolServer.API.Controllers;

[Authorize]
[ApiController]
[Route("api/lessons")]
public class LessonsController : Controller
{
    private readonly LessonsService lessonsService;
    private readonly FavouriteService favouriteService;
    private readonly JWTOptions jWTOptions;
    private readonly IJWTProvider jWTProvider;

    public LessonsController(LessonsService lessonsService, FavouriteService favouriteService, IOptions<JWTOptions> jWTOptions, IJWTProvider jWTProvider)
    {
        this.lessonsService = lessonsService;
        this.favouriteService = favouriteService;
        this.jWTOptions = jWTOptions.Value;
        this.jWTProvider = jWTProvider;
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


    [HttpPost("add_to_favourite/{id:int}")]
    public async Task<ActionResult> AddToFavourite(int id)
    {
        string token = Request.Cookies[jWTOptions.JWTCookieName] ?? string.Empty;
        string username = jWTProvider.GetUsernameFromToken(token);
        try
        {
            await favouriteService.AddUserFavourite(username, id);
            return Ok();
        }
        catch (UserNotFoundException)
        {
            return NotFound(new { msg = "user not found" });
        }
        catch (LessonNotFoundException)
        {
            return NotFound(new { msg = "lesson not found" });
        }
        catch(Exception ex)
        {
            return StatusCode(500, ex.Message);
        }

    }
    [HttpDelete("remove_from_favourite/{id:int}")]
    public async Task<ActionResult> RemoveFromFavourite(int id)
    {
        string token = Request.Cookies[jWTOptions.JWTCookieName] ?? string.Empty;
        string username = jWTProvider.GetUsernameFromToken(token);
        try
        {
            await favouriteService.RemoveUserFavourite(username, id);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }

    }



}
