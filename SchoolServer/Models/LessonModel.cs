using System.ComponentModel.DataAnnotations;
namespace SchoolServer.API.Models;

public class LessonModel
{
    [Required] public string Title { get; set; } = string.Empty;
    [Required] public string ImageUrl { get; set; } = string.Empty;
    [Required] public string ContentUrl { get; set; } = string.Empty;
}
