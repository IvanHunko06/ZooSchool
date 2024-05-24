using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using SchoolServer.Infrastructure.Authentification;
using SchoolServer.Application.Services;
using SchoolServer.Application.Exceptions;
namespace SchoolServer.API.Controllers;

[Route("resources")]
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
            return Conflict();
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
}
