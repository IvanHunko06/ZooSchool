using Microsoft.EntityFrameworkCore;
using SchoolServer.DataAccess.SQLServer.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace SchoolServer.DataAccess.SQLServer.Configurations;

public class TestAnswerConfiguration :IEntityTypeConfiguration<TestAnswerEntity>
{
    public void Configure(EntityTypeBuilder<TestAnswerEntity> builder)
    {
        builder.ToTable("TestAnswers");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AnswerValue)
           .IsRequired()
           .HasMaxLength(50)
           .IsUnicode(true);

        builder.Property(x => x.QuestionId)
            .IsRequired();

        builder.HasOne(x => x.Attempt)
            .WithMany(a=>a.Answers)
            .HasForeignKey(x=>x.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
