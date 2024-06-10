using Microsoft.EntityFrameworkCore;
using SchoolServer.DataAccess.SQLServer.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace SchoolServer.DataAccess.SQLServer.Configurations;


public class FavouriteConfiguration : IEntityTypeConfiguration<FavoriteEntityl>
{
    public void Configure(EntityTypeBuilder<FavoriteEntityl> builder)
    {
        builder.ToTable("Favourites");
        builder.HasKey(r => new {r.UserId, r.LessonId});

        builder.HasOne(u => u.User)
            .WithMany(a => a.Favorites)
            .HasForeignKey(a => a.UserId);

        builder.HasOne(u=>u.Lesson)
            .WithMany(a=>a.UsersFavoriets)
            .HasForeignKey(a=>a.LessonId);
    }
}
