using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Taskmony.Models.Tasks;

namespace Taskmony.Data.Configurations;

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Assignment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasValueGenerator<GuidValueGenerator>();
        
        builder.Property(a => a.CreatedAt)
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.HasOne(a => a.Assignee)
            .WithMany(u => u.AssignedTo)
            .HasForeignKey(a => a.AssigneeId);

        builder.HasOne(a => a.AssignedBy)
            .WithMany(u => u.AssignedBy)
            .HasForeignKey(a => a.AssignedById);
    }
}