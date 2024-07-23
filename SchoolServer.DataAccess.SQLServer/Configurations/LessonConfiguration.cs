using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolServer.DataAccess.SQLServer.Models;

namespace SchoolServer.DataAccess.SQLServer.Configurations
{
    public class LessonConfiguration : IEntityTypeConfiguration<LessonEntity>
    {
        public void Configure(EntityTypeBuilder<LessonEntity> builder)
        {
            builder.ToTable("Lessons");
            builder.HasKey(l=>l.Id);

            builder.Property(l=>l.ContentUrl).IsRequired().IsUnicode(false).HasMaxLength(100);
            builder.Property(l=>l.TitleImageUrl).IsRequired().IsUnicode(false).HasMaxLength(100);
            builder.Property(l=>l.Title).IsRequired().HasMaxLength(100);
        }
    }
}
