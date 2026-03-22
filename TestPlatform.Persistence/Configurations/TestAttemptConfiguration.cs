using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestPlatform.Domain.Aggregates;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Persistence.Configurations;

internal sealed class TestAttemptConfiguration : IEntityTypeConfiguration<TestAttempt>
{
    public void Configure(EntityTypeBuilder<TestAttempt> builder)
    {
        builder.ToTable("TestAttempts");
        builder.HasKey(attempt => attempt.Id);

        builder.Property(attempt => attempt.UserId).IsRequired();
        builder.Property(attempt => attempt.TestId).IsRequired();
        builder.Property(attempt => attempt.Score).IsRequired();
        builder.Property(attempt => attempt.IsEvaluated).IsRequired();

        builder.OwnsMany(attempt => attempt.Answers, answerBuilder =>
        {
            answerBuilder.ToTable("UserAnswers");
            answerBuilder.WithOwner().HasForeignKey("TestAttemptId");
            answerBuilder.Property(answer => answer.QuestionId).IsRequired();
            answerBuilder.Property(answer => answer.SelectedOptionId).IsRequired();
            answerBuilder.HasKey("TestAttemptId", nameof(UserAnswer.QuestionId));
        });
    }
}
