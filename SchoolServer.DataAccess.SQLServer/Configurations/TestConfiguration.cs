using Microsoft.EntityFrameworkCore;
using SchoolServer.DataAccess.SQLServer.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace SchoolServer.DataAccess.SQLServer.Configurations;

public class TestConfiguration : IEntityTypeConfiguration<TestEntity>
{
    public void Configure(EntityTypeBuilder<TestEntity> builder)
    {
        builder.ToTable("Tests");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AnswersFile)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(x => x.ContentUrl)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(false);
        builder.Property(x => x.Title)
            .IsRequired()
            .IsUnicode(true);

        builder
            .HasMany(t => t.Atempts)
            .WithOne(a => a.Test)
            .HasForeignKey(a => a.TestId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.Title).IsUnique();

    }
}
