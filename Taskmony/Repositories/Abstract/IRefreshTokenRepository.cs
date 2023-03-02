using Taskmony.Models;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Repositories.Abstract;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    
    Task<RefreshToken?> GetAsync(string token);

    Task<IEnumerable<RefreshToken>> GetByUserIdAsync(Guid userId);

    Task<bool> SaveChangesAsync();
}