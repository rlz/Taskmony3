using Taskmony.Models;

namespace Taskmony.Repositories.Abstract;

public interface IMessageTemplateRepository
{
    Task<MessageTemplate?> GetByNameAsync(string name);
}