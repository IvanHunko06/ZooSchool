using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using SchoolServer.Infrastructure.Authentification;
using SchoolServer.Application.Services;
using SchoolServer.Application.Exceptions;
namespace SchoolServer.API.Controllers;

[Route("resources")]
[ApiController]
[Authorize]
public class ResourcesController : Controller
{
    private readonly ResourceServices resourceServices;

    public ResourcesController(ResourceServices resourceServices)
    {
        this.resourceServices = resourceServices;
    }

    [HttpGet("images/get/{imageName}")]
    [AllowAnonymous]
    public  IActionResult GetImage(string imageName)
    {
        try
        {
            var image = resourceServices.GetResource("Images", imageName);
            string type = string.Empty;
            if (imageName.Contains("png"))
                type = "png";
            if (imageName.Contains("jpeg"))
                type = "jpeg";
            return File(image, $"image/{type}");
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
        
    }
    [HasPermission(Core.Enums.Permission.CreateResource)]
    [HttpPost("images/post")]
    public async Task<IActionResult> PostImage(IFormFile file)
    {
        try
        {
            await resourceServices.CreateResource("Images", file, new string[] { "png", "jpeg" });
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (FileExistException)
        {
            return Conflict();
        }
        catch(Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

    }
    [HttpDelete("images/delete/{imageFile}")]
    [HasPermission(Core.Enums.Permission.DeleteResource)]
    public  IActionResult DeleteImage(string imageFile)
    {
        try
        {
            resourceServices.DeleteResource("Images", imageFile);
            return Ok();
        }catch(FileNotFoundException) 
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

    }

    [HttpGet("images")]
    [HasPermission(Core.Enums.Permission.ViewFiles)]
    public  IActionResult GetImageFiles()
    {
        return Ok(resourceServices.GetAllFiles("Images"));
    }
    [HttpGet("content")]
    [HasPermission(Core.Enums.Permission.ViewFiles)]
    public IActionResult GetContentFiles()
    {
        return Ok(resourceServices.GetAllFiles("Contents"));
    }

    [AllowAnonymous]
    [HttpGet("content/get/{contentName}")]
    public IActionResult GetContent(string contentName)
    {
        try
        {
            var content = resourceServices.GetResource("Contents", contentName);
            return File(content, $"text/json");
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }

    }

    [HasPermission(Core.Enums.Permission.CreateResource)]
    [HttpPost("content/post")]
    public async Task<IActionResult> PostContent(IFormFile file)
    {
        try
        {
            await resourceServices.CreateResource("Contents", file, new string[] { "json" });
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (FileExistException)
        {
            return Conflict("file already exist in server");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

    }

    [HttpDelete("content/delete/{contentFile}")]
    [HasPermission(Core.Enums.Permission.DeleteResource)]
    public IActionResult DeleteContent(string contentFile)
    {
        try
        {
            resourceServices.DeleteResource("Contents", contentFile);
            return Ok();
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

    }

    [HttpGet("tests_content")]
    [HasPermission(Core.Enums.Permission.ViewFiles)]
    public IActionResult GetTestsContentFiles()
    {
        return Ok(resourceServices.GetAllFiles("TestsContents"));
    }

    [AllowAnonymous]
    [HttpGet("tests_content/get/{contentName}")]
    public IActionResult GetTestsContent(string contentName)
    {
        try
        {
            var content = resourceServices.GetResource("TestsContents", contentName);
            return File(content, $"text/json");
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }

    }

    [HasPermission(Core.Enums.Permission.CreateResource)]
    [HttpPost("tests_content/post")]
    public async Task<IActionResult> PostTestsContent(IFormFile file)
    {
        try
        {
            await resourceServices.CreateResource("TestsContents", file, new string[] { "json" });
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (FileExistException)
        {
            return Conflict("file already exist in server");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

    }

    [HttpDelete("tests_content/delete/{contentFile}")]
    [HasPermission(Core.Enums.Permission.DeleteResource)]
    public IActionResult DeleteTestsContent(string contentFile)
    {
        try
        {
            resourceServices.DeleteResource("TestsContents", contentFile);
            return Ok();
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

    }

    [HttpGet("tests_answer")]
    [HasPermission(Core.Enums.Permission.ViewFiles)]
    public IActionResult GetTestsAnswersFiles()
    {
        return Ok(resourceServices.GetAllFiles("TestsAnswers"));
    }

    [AllowAnonymous]
    [HttpGet("tests_answer/get/{contentName}")]
    public IActionResult GetTestsAnswer(string contentName)
    {
        try
        {
            var content = resourceServices.GetResource("TestsAnswers", contentName);
            return File(content, $"text/json");
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }

    }

    [HasPermission(Core.Enums.Permission.CreateResource)]
    [HttpPost("tests_answer/post")]
    public async Task<IActionResult> PostTestsAnswer(IFormFile file)
    {
        try
        {
            await resourceServices.CreateResource("TestsAnswers", file, new string[] { "json" });
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (FileExistException)
        {
            return Conflict("file already exist in server");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

    }

    [HttpDelete("tests_answer/delete/{contentFile}")]
    [HasPermission(Core.Enums.Permission.DeleteResource)]
    public IActionResult DeleteTestsAnswer(string contentFile)
    {
        try
        {
            resourceServices.DeleteResource("TestsAnswers", contentFile);
            return Ok();
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

    }
}
