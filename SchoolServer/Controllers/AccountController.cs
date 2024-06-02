using Microsoft.AspNetCore.Mvc;
using SchoolServer.Models;
using SchoolServer.Application.Services;
using SchoolServer.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SchoolServer.Core.Models;
using SchoolServer.Infrastructure.Authentification;
using SchoolServer.Application.Interfaces.Auth;
using SchoolServer.API.Models;
namespace SchoolServer.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly UsersServices usersServices;
    private readonly IJWTProvider jWTProvider;
    private readonly JWTOptions jWTOptions;

    public AccountController(ILogger<AccountController> logger, UsersServices usersServices, IOptions<JWTOptions> jWTOptions, IJWTProvider jWTProvider)
    {
        _logger = logger;
        this.usersServices = usersServices;
        this.jWTProvider = jWTProvider;
        this.jWTOptions = jWTOptions.Value;
    }

    [HttpPost("create_account")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountModel account)
    {
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
        catch (IncorrectPasswordException)
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

    [HasPermission(Core.Enums.Permission.GetOwnAccount)]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        string token = Request.Cookies[jWTOptions.JWTCookieName] ?? string.Empty;
        string username = jWTProvider.GetUsernameFromToken(token);
        User user;
        try
        {
            user = await usersServices.GetByUsername(username);
            return Ok(new { username = user.Username});
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
    [HasPermission(Core.Enums.Permission.GetAnyAccount)]
    [HttpGet("{targetUsername}")]
    public async Task<IActionResult> Get(string targetUsername)
    {
        User user;
        try
        {
            user = await usersServices.GetByUsername(targetUsername);
            return Ok(new { username = user.Username, passwordHash = user.PasswordHash });
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
        }

    }



    [HasPermission(Core.Enums.Permission.DeleteOwnAccount)]
    [HttpDelete("delete_account")]
    public async Task<IActionResult> DeleteAccount()
    {
        string token = Request.Cookies[jWTOptions.JWTCookieName] ?? string.Empty;
        string username = jWTProvider.GetUsernameFromToken(token);
        try
        {
            await usersServices.DeleteAccount(username);
            return Ok();
        }
        catch(Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
        }

    }

    [HasPermission(Core.Enums.Permission.DeleteAnyAccount)]
    [HttpDelete("delete_account/{targetUsername}")]
    public async Task<IActionResult> DeleteAccount(string targetUsername)
    {
        try
        {
            await usersServices.DeleteAccount(targetUsername);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
        }
    }

    [HttpPatch("change_own_password")]
    [HasPermission(Core.Enums.Permission.ChangeOwnPassword)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel changePasswordModel)
    {
        try
        {
            string token = Request.Cookies[jWTOptions.JWTCookieName] ?? string.Empty;
            string username = jWTProvider.GetUsernameFromToken(token);
            await usersServices.ChangePassword(username, changePasswordModel.CurrentPassword, changePasswordModel.NewPassword);
            return Ok(new {code = 0});
        }
        catch(UserNotFoundException)
        {
            return NotFound(new {code = 1});
        }
        catch (IncorrectPasswordException)
        {
            return BadRequest(new { code = 2 });
        }
        catch(Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
        }
    }
    [HttpPatch("change_any_password")]
    [HasPermission(Core.Enums.Permission.ChangeAnyPassword)]
    public async Task<IActionResult> ChangePassword([FromBody] SetPasswordModel changePasswordModel)
    {
        try
        {
            await usersServices.ChangePassword(changePasswordModel.Username, changePasswordModel.NewPassword);
            return Ok(new { code = 0 });
        }
        catch (UserNotFoundException)
        {
            return NotFound(new { code = 1 });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
        }
    }
}
