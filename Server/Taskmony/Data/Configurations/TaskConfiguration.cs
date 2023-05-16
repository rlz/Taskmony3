using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Taskmony.Models.Tasks;
using Task = Taskmony.Models.Tasks.Task;

namespace Taskmony.Data.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasValueGenerator<GuidValueGenerator>();

        builder.Property(t => t.CreatedAt)
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.Property(t => t.StartAt)
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.HasOne(t => t.Assignment)
            .WithOne(a => a.Task)
            .HasForeignKey<Assignment>(a => a.TaskId);

        builder.HasOne(t => t.RecurrencePattern)
            .WithOne(r => r.Task)
            .HasForeignKey<RecurrencePattern>(r => r.TaskId);

        builder.OwnsOne(t => t.Description, b =>
        {
            b.Property(l => l.Value)
                .HasColumnName(nameof(Task.Description))
                .IsRequired();
        }).Navigation(t => t.Description).IsRequired();

        builder.OwnsOne(t => t.DeletedAt, b =>
        {
            b.Property(l => l.Value)
                .HasColumnName(nameof(Task.DeletedAt));
        });

        builder.OwnsOne(t => t.CompletedAt, b =>
        {
            b.Property(l => l.Value)
                .HasColumnName(nameof(Task.CompletedAt));
        });

        builder.Ignore(t => t.ActionItemType);
        builder.Ignore(t => t.Notifications);
    }
}