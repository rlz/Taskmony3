using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Taskmony.Models.Directions;

namespace Taskmony.Data.Configurations;

public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.Property(d => d.CreatedAt)
            .HasDefaultValueSql("now()")
            .IsRequired();
    }
}