using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Taskmony.Models.Subscriptions;

namespace Taskmony.Data.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.UseTptMappingStrategy();

        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.User)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.Property(s => s.Id)
            .HasValueGenerator<GuidValueGenerator>();
    }
}