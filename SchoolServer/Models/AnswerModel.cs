using System.ComponentModel.DataAnnotations;

namespace SchoolServer.API.Models
{
    public class AnswerModel
    {
        [Required] public int QuestionId {  get; set; }
        [Required] public string AnswerValue { get; set; } = string.Empty;
    }
}
