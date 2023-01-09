namespace Taskmony.Repositories;

public interface IIdeaRepository
{
    Task<bool> SaveChangesAsync();
}