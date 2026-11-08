using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestPlatform.Domain.Aggregates;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Persistence.Configurations;

internal sealed class TestConfiguration : IEntityTypeConfiguration<Test>
{
    public void Configure(EntityTypeBuilder<Test> builder)
    {
        builder.ToTable("Tests");
        builder.HasKey(test => test.Id);

        builder.Property(test => test.CreatedByUserId)
            .IsRequired();

        builder.HasIndex(test => test.CreatedByUserId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(test => test.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(test => test.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Navigation(test => test.QuestionReferences)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindNavigation(nameof(Test.QuestionReferences))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(test => test.QuestionReferences, referenceBuilder =>
        {
            referenceBuilder.ToTable("TestQuestionReferences");
            referenceBuilder.WithOwner().HasForeignKey("TestId");
            referenceBuilder.Property(reference => reference.QuestionId).IsRequired();
            referenceBuilder.Property(reference => reference.Order).IsRequired();
            referenceBuilder.HasKey("TestId", nameof(TestQuestionReference.QuestionId));
            referenceBuilder.HasIndex("TestId", nameof(TestQuestionReference.Order)).IsUnique();
        });
    }
}
