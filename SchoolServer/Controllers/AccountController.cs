using Microsoft.AspNetCore.Mvc;
using SchoolServer.Models;
namespace SchoolServer.Controllers
{
    [Route("api/acount")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpPost("create_account")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountModel account)
        {
            return Ok(new { code = 0 });
        }
    }
}
