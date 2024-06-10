namespace SchoolServer.DataAccess.SQLServer.Models;

public class LessonEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string TitleImageUrl { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string ContentUrl { get; set; }

    public List<FavoriteEntityl> UsersFavoriets { get; set; }
}
