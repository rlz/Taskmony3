using Taskmony.Models;

namespace Taskmony.Repositories.Abstract;

public interface IVerificationTokenRepository
{
    Task<VerificationToken?> GetAsync(Guid userId, Guid token);

    void Add(VerificationToken verificationToken);
    
    void DeleteByUserId(Guid userId);

    Task<bool> SaveChangesAsync();
}