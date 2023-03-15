using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Taskmony.Models.Comments;

namespace Taskmony.Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.UseTptMappingStrategy();

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasValueGenerator<GuidValueGenerator>();

        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("now()")
            .IsRequired();
        
        builder.OwnsOne(c => c.Text, b =>
        {
            b.Property(l => l.Value)
                .HasColumnName(nameof(Comment.Text))
                .IsRequired();
        }).Navigation(c => c.Text).IsRequired();

        builder.OwnsOne(c => c.DeletedAt, b =>
        {
            b.Property(l => l.Value)
                .HasColumnName(nameof(Comment.DeletedAt));
        });

        builder.Ignore(c => c.ActionItemType);
    }
}