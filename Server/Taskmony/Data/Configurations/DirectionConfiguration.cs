using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Taskmony.Models.Directions;

namespace Taskmony.Data.Configurations;

public class DirectionConfiguration : IEntityTypeConfiguration<Direction>
{
    public void Configure(EntityTypeBuilder<Direction> builder)
    {
        builder.HasKey(d => d.Id);

        builder.HasMany(d => d.Tasks)
            .WithOne(t => t.Direction);

        builder.HasMany(d => d.Ideas)
            .WithOne(i => i.Direction);
        
        builder.Property(d => d.Id)
            .HasValueGenerator<GuidValueGenerator>();

        builder.Property(d => d.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");
        
        builder.OwnsOne(d => d.DeletedAt, b =>
        {
            b.Property(l => l.Value)
                .HasColumnName(nameof(Direction.DeletedAt));
        });
        
        builder.OwnsOne(d => d.Name, b =>
        {
            b.Property(l => l.Value)
                .HasColumnName(nameof(Direction.Name))
                .IsRequired();
        }).Navigation(d => d.Name).IsRequired();

        builder.Ignore(d => d.Notifications);
    }
}