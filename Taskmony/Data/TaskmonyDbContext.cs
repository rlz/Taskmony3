using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Taskmony.Models;
using Taskmony.Models.Comments;
using Taskmony.Models.Subscriptions;

namespace Taskmony.Data;

public class TaskmonyDbContext : DbContext
{
    public TaskmonyDbContext(DbContextOptions<TaskmonyDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Direction> Directions => Set<Direction>();

    public DbSet<Idea> Ideas => Set<Idea>();

    public DbSet<Models.Task> Tasks => Set<Models.Task>();

    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    public DbSet<TaskSubscription> TaskSubscriptions => Set<TaskSubscription>();

    public DbSet<IdeaSubscription> IdeaSubscriptions => Set<IdeaSubscription>();

    public DbSet<Comment> Comments => Set<Comment>();

    public DbSet<TaskComment> TaskComments => Set<TaskComment>();

    public DbSet<IdeaComment> IdeaComments => Set<IdeaComment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.Tasks)
            .WithOne(t => t.CreatedBy)
            .HasForeignKey(t => t.CreatedById);

        modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .HasValueGenerator<GuidValueGenerator>();

        modelBuilder.Entity<User>()
            .Property(u => u.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<User>()
            .HasMany(u => u.Tasks)
            .WithOne(t => t.CreatedBy)
            .HasForeignKey(t => t.CreatedById);

        modelBuilder.Entity<User>()
            .HasMany(u => u.AssignedTasks)
            .WithOne(t => t.Assignee)
            .HasForeignKey(t => t.AssigneeId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.OwnDirections)
            .WithOne(d => d.CreatedBy)
            .HasForeignKey(d => d.CreatedById);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Directions)
            .WithMany(d => d.Members);

        modelBuilder.Entity<User>()
            .Property(u => u.Password)
            .HasColumnName("PasswordHash");

        modelBuilder.Entity<Models.Task>()
            .Property(u => u.Id)
            .HasValueGenerator<GuidValueGenerator>();

        modelBuilder.Entity<Models.Task>()
            .Property(t => t.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<Idea>()
            .Property(u => u.Id)
            .HasValueGenerator<GuidValueGenerator>();

        modelBuilder.Entity<Idea>()
            .Property(i => i.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<Direction>()
            .Property(u => u.Id)
            .HasValueGenerator<GuidValueGenerator>();

        modelBuilder.Entity<Direction>()
            .Property(d => d.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<Direction>()
            .HasMany(d => d.Tasks)
            .WithOne(t => t.Direction);

        modelBuilder.Entity<Direction>()
            .HasMany(d => d.Ideas)
            .WithOne(i => i.Direction);

        modelBuilder.Entity<Comment>().UseTptMappingStrategy();

        modelBuilder.Entity<Comment>()
            .Property(u => u.Id)
            .HasValueGenerator<GuidValueGenerator>();

        modelBuilder.Entity<Comment>()
            .Property(c => c.CreatedAt)
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<Notification>()
            .Property(u => u.Id)
            .HasValueGenerator<GuidValueGenerator>();

        modelBuilder.Entity<Notification>()
            .Property(c => c.ModifiedAt)
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<Subscription>().UseTptMappingStrategy();

        modelBuilder.Entity<Subscription>()
            .Property(c => c.SubscribedAt)
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<Subscription>()
            .Property(u => u.Id)
            .HasValueGenerator<GuidValueGenerator>();

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.User)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.UserId);
    }
}