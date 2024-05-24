﻿using System.ComponentModel.DataAnnotations;
namespace SchoolServer.Models;

public class LoginToAccountModel
{
    [Required] public string Login { get; set; } = string.Empty;
    [Required]  public string Password { get; set; } = string.Empty;
}
