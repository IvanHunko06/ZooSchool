namespace SchoolServer.Core.Models;

public class Lesson
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string ContentUrl { get; set; } = string.Empty;
}
