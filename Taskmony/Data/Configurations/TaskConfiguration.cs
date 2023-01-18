using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Taskmony.Data.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<Models.Task>
{
    public void Configure(EntityTypeBuilder<Models.Task> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasValueGenerator<GuidValueGenerator>();

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.Property(t => t.StartAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.Ignore(t => t.ActionItemType);
        builder.Ignore(t => t.NumberOfRepetitions);
        builder.Ignore(t => t.Notifications);

        builder.Property(t => t.Description).IsRequired();
    }
}