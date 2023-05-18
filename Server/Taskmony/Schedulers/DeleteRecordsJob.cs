using Microsoft.Extensions.Options;
using Quartz;
using Taskmony.Repositories.Abstract;

namespace Taskmony.Schedulers;

[DisallowConcurrentExecution]
public class DeleteRecordsJob : IJob
{
    private readonly IDirectionRepository _directionRepository;
    private readonly ILogger<DeleteRecordsJob> _logger;
    private readonly DeleteRecordsJobOptions _options;

    public DeleteRecordsJob(IDirectionRepository directionRepository, ILogger<DeleteRecordsJob> logger,
        IOptions<DeleteRecordsJobOptions> options)
    {
        _directionRepository = directionRepository;
        _logger = logger;
        _options = options.Value;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Deleting old soft deleted records");

        _directionRepository.HardDeleteSoftDeletedDirectionsWithChildren(DateTime.UtcNow.AddDays(-_options.DeleteAfterDays.GetValueOrDefault(30)));

        await _directionRepository.SaveChangesAsync();
    }
}