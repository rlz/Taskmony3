using Microsoft.EntityFrameworkCore;
using Taskmony.Data;
using Taskmony.Models;
using Taskmony.Repositories.Abstract;

namespace Taskmony.Repositories;

public class MessageTemplateRepository : BaseRepository<MessageTemplate>, IMessageTemplateRepository
{
    public MessageTemplateRepository(IDbContextFactory<TaskmonyDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<MessageTemplate?> GetByNameAsync(string name)
    {
        return await Context.MessageTemplates.FirstOrDefaultAsync(x => x.Name == name);
    }
}
