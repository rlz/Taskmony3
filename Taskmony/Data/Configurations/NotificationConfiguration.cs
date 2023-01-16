using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Taskmony.Models.Notifications;

namespace Taskmony.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .HasValueGenerator<GuidValueGenerator>();

        builder.Property(n => n.ModifiedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.Ignore(n => n.ActionItem);

        builder.Property(n => n.NotifiableType).IsRequired();
        builder.Property(n => n.ActionType).IsRequired();
    }
}