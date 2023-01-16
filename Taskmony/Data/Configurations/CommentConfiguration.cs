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
            .HasDefaultValueSql("now()");

        builder.Ignore(c => c.ActionItemType);

        builder.Property(c => c.Text).IsRequired();
        builder.Property(c => c.CreatedAt).IsRequired();
    }
}