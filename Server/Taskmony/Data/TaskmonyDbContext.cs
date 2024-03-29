using Microsoft.EntityFrameworkCore;
using Taskmony.Data.Configurations;
using Taskmony.Models.Comments;
using Taskmony.Models.Directions;
using Taskmony.Models.Ideas;
using Taskmony.Models.Notifications;
using Taskmony.Models.Subscriptions;
using Taskmony.Models.Tasks;
using Taskmony.Models.Users;
using Task = Taskmony.Models.Tasks.Task;

namespace Taskmony.Data;

public class TaskmonyDbContext : DbContext
{
    public TaskmonyDbContext(DbContextOptions<TaskmonyDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Direction> Directions => Set<Direction>();

    public DbSet<Membership> Memberships => Set<Membership>();

    public DbSet<Idea> Ideas => Set<Idea>();

    public DbSet<Task> Tasks => Set<Task>();

    public DbSet<Assignment> Assignments => Set<Assignment>();

    public DbSet<RecurrencePattern> RecurrencePatterns => Set<RecurrencePattern>();

    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    public DbSet<TaskSubscription> TaskSubscriptions => Set<TaskSubscription>();

    public DbSet<IdeaSubscription> IdeaSubscriptions => Set<IdeaSubscription>();

    public DbSet<Comment> Comments => Set<Comment>();

    public DbSet<TaskComment> TaskComments => Set<TaskComment>();

    public DbSet<IdeaComment> IdeaComments => Set<IdeaComment>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new DirectionConfiguration());
        modelBuilder.ApplyConfiguration(new MembershipConfiguration());
        modelBuilder.ApplyConfiguration(new IdeaConfiguration());
        modelBuilder.ApplyConfiguration(new TaskConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionConfiguration());
        modelBuilder.ApplyConfiguration(new CommentConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new AssignmentConfiguration());
        modelBuilder.ApplyConfiguration(new RecurrencePatternConfiguration());
    }
}