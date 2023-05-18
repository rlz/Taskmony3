using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Taskmony.Models.Ideas;

namespace Taskmony.Data.Configurations;

public class IdeaConfiguration : IEntityTypeConfiguration<Idea>
{
    public void Configure(EntityTypeBuilder<Idea> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasValueGenerator<GuidValueGenerator>();
        
        builder.Property(i => i.CreatedAt)
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.OwnsOne(i => i.Description, b =>
        {
            b.Property(l => l.Value)
                .HasColumnName(nameof(Idea.Description))
                .IsRequired();
        }).Navigation(i => i.Description).IsRequired();
        
        builder.OwnsOne(i => i.Details, b =>
        {
            b.Property(l => l.Value)
                .HasColumnName(nameof(Idea.Details))
                .IsRequired();
        });
        
        builder.OwnsOne(i => i.DeletedAt, b =>
        {
            b.Property(l => l.Value)
                .HasColumnName(nameof(Idea.DeletedAt));
        });
        
        builder.OwnsOne(i => i.ReviewedAt, b =>
        {
            b.Property(l => l.Value)
                .HasColumnName(nameof(Idea.ReviewedAt));
        });
        
        builder.Property(i => i.Generation).IsRequired();

        builder.Ignore(i => i.ActionItemType);
        builder.Ignore(i => i.Notifications);
    }
}