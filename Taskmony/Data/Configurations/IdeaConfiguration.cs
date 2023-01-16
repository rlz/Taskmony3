using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Taskmony.Models;

namespace Taskmony.Data.Configurations;

public class IdeaConfiguration : IEntityTypeConfiguration<Idea>
{
    public void Configure(EntityTypeBuilder<Idea> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasValueGenerator<GuidValueGenerator>();

        builder.Property(i => i.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.Ignore(i => i.ActionItemType);
        builder.Ignore(i => i.Notifications);

        builder.Property(i => i.Description).IsRequired();
        builder.Property(i => i.CreatedAt).IsRequired();
        builder.Property(i => i.Generation).IsRequired();
    }
}