using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestPlatform.Domain.Aggregates;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Persistence.Configurations;

internal sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");
        builder.HasKey(question => question.Id);

        builder.Property(question => question.CreatedByUserId)
            .IsRequired();

        builder.HasIndex(question => question.CreatedByUserId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(question => question.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(question => question.Text)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(question => question.ExplanationText)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(question => question.CorrectOptionId)
            .IsRequired();

        builder.Property(question => question.IsDeleted)
            .IsRequired();

        builder.Navigation(question => question.Options)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindNavigation(nameof(Question.Options))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(question => question.Options, optionBuilder =>
        {
            optionBuilder.ToTable("QuestionOptions");
            optionBuilder.WithOwner().HasForeignKey("QuestionId");
            optionBuilder.HasKey(option => option.Id);

            optionBuilder.Property(option => option.Text)
                .HasMaxLength(200)
                .IsRequired();

            optionBuilder.Property(option => option.Order)
                .IsRequired();

            optionBuilder.HasIndex("QuestionId", nameof(Option.Order)).IsUnique();
        });
    }
}
