using System.ComponentModel.DataAnnotations;

namespace SchoolServer.API.Models;

public class CreateTestModel
{
    [Required] public string Title {  get; set; } = string.Empty;
    [Required] public string ContentUrl {  get; set; } = string.Empty;
    [Required] public string AnswersFile { get; set; } = string.Empty;
}
