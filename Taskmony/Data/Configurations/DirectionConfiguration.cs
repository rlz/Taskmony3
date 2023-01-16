using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Taskmony.Models;

namespace Taskmony.Data.Configurations;

public class DirectionConfiguration : IEntityTypeConfiguration<Direction>
{
    public void Configure(EntityTypeBuilder<Direction> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasValueGenerator<GuidValueGenerator>();

        builder.Property(d => d.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.HasMany(d => d.Tasks)
            .WithOne(t => t.Direction);

        builder.HasMany(d => d.Ideas)
            .WithOne(i => i.Direction);

        builder.Ignore(d => d.Notifications);

        builder.Property(d => d.Name).IsRequired();
    }
}