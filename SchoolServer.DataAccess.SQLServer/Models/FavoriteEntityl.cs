namespace SchoolServer.DataAccess.SQLServer.Models;

public class FavoriteEntityl
{
    public int LessonId {  get; set; }
    public LessonEntity Lesson { get; set; }
    public int UserId { get; set; }
    public UserEntity User { get; set; }
    public DateTime AddedTime { get; set; }
}
