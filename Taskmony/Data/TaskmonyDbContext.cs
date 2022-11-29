using Microsoft.EntityFrameworkCore;
using Taskmony.Models;

namespace Taskmony.Data;

public class TaskmonyDbContext : DbContext
{
    public TaskmonyDbContext(DbContextOptions<TaskmonyDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
}