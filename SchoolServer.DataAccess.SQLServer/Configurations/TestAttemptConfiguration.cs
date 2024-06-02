using Microsoft.EntityFrameworkCore;
using SchoolServer.DataAccess.SQLServer.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace SchoolServer.DataAccess.SQLServer.Configurations;

public class TestAttemptConfiguration : IEntityTypeConfiguration<TestAttemptEntity>
{
    public void Configure(EntityTypeBuilder<TestAttemptEntity> builder)
    {
        builder.ToTable("TestAttempts");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AttemptTime)
           .IsRequired(true);


        builder.HasOne(x=>x.User)
            .WithMany(a=>a.TestAttempts)
            .HasForeignKey(x=>x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x=>x.Answers)
            .WithOne(a=>a.Attempt)
            .HasForeignKey(a=>a.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x=>x.Test)
            .WithMany(t=>t.Atempts)
            .HasForeignKey(t=>t.TestId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
