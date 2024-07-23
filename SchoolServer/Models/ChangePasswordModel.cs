using System.ComponentModel.DataAnnotations;

namespace SchoolServer.API.Models;

public class ChangePasswordModel
{
    [Required] public string CurrentPassword { get; set; }
    [Required] public string NewPassword { get; set; }
}
