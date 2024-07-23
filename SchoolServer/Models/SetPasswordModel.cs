using System.ComponentModel.DataAnnotations;

namespace SchoolServer.API.Models
{
    public class SetPasswordModel
    {
        [Required] public string Username { get; set; }
        [Required] public string NewPassword { get; set; }
    }
}
