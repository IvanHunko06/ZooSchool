using Microsoft.AspNetCore.Mvc;
using SchoolServer.Models;
using SchoolServer.Application.Services;
using SchoolServer.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SchoolServer.Core.Models;
using SchoolServer.Infrastructure.Authentification;
namespace SchoolServer.Controllers;

[ApiController]
[Route("api/acount")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly UsersServices usersServices;
    private readonly JWTOptions jWTOptions;

    public AccountController(ILogger<AccountController> logger, UsersServices usersServices, IOptions<JWTOptions> jWTOptions)
    {
        _logger = logger;
        this.usersServices = usersServices;
        this.jWTOptions = jWTOptions.Value;
    }

    [HttpPost("create_account")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountModel account)
    {
        if (string.IsNullOrEmpty(account.Login))
            return BadRequest(new { code = -1 });
        if (string.IsNullOrEmpty(account.Password))
            return BadRequest(new { code = -2 });

        _logger.LogInformation($"Create account: Username: {account.Login},  Password: {account.Password}");
        try
        {
            await usersServices.Register(account.Login, account.Password);
            return StatusCode(StatusCodes.Status201Created, new { code = 0 });
        }
        catch (UserAlreadyExistException)
        {
            return Conflict(new { code = 1 });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
        }

    }
    [HttpPost("login_to_account")]
    public async Task<IActionResult> LoginToAccount([FromBody] LoginToAccountModel account)
    {
        if (string.IsNullOrEmpty(account.Login))
            return BadRequest(new { code = -1 });
        if (string.IsNullOrEmpty(account.Password))
            return BadRequest(new { code = -2 });
        string token = string.Empty;
        try
        {
            token = await usersServices.Login(account.Login, account.Password);
            Response.Cookies.Append(jWTOptions.JWTCookieName, token);
            return Ok(new { code = 0 });
        }
        catch (UserNotFoundException)
        {
            return Unauthorized(new { code = 1 });
        }
        catch (IncorrectLoginPasswordException)
        {
            return Unauthorized(new { code = 2 });
        }
        catch (UserBannedException)
        {
            return Unauthorized(new { code = 3 });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
        }


    }
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        string token = Request.Cookies[jWTOptions.JWTCookieName] ?? string.Empty;
        User user;
        try
        {
            user = await usersServices.GetUserByJWTToken(token);
            return Ok(new { username = user.UserName, passwordHash = user.PasswordHash, id = user.Id });
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
        catch(Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
        }
        
    }

}
