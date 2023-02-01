using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Taskmony.Models;

namespace Taskmony.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasValueGenerator<GuidValueGenerator>();

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.HasMany(u => u.Tasks)
            .WithOne(t => t.CreatedBy)
            .HasForeignKey(t => t.CreatedById)
            .IsRequired();

        builder.HasMany(u => u.AssignedTasks)
            .WithOne(t => t.Assignee)
            .HasForeignKey(t => t.AssigneeId);

        builder.HasMany(u => u.OwnDirections)
            .WithOne(d => d.CreatedBy)
            .HasForeignKey(d => d.CreatedById)
            .IsRequired();

        builder.HasMany(u => u.Directions)
            .WithMany(d => d.Members)
            .UsingEntity<Membership>();

        builder.Property(u => u.Password)
            .HasColumnName("PasswordHash")
            .IsRequired();

        builder.OwnsOne(u => u.Login, b =>
        {
            b.Property(l => l.Value)
                .HasColumnName(nameof(User.Login))
                .IsRequired();
        }).Navigation(u => u.Login).IsRequired();

        builder.OwnsOne(u => u.DisplayName, b =>
        {
            b.Property(d => d.Value)
                .HasColumnName(nameof(User.DisplayName))
                .IsRequired();
        }).Navigation(u => u.DisplayName).IsRequired();

        builder.OwnsOne(u => u.Email, b =>
        {
            b.Property(e => e.Value)
                .HasColumnName(nameof(User.Email))
                .IsRequired();
        }).Navigation(u => u.Email).IsRequired();

        builder.Ignore(u => u.ActionItemType);
    }
}