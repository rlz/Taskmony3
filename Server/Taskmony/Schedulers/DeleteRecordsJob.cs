using Microsoft.Extensions.Options;
using Quartz;
using Taskmony.Repositories.Abstract;

namespace Taskmony.Schedulers;

[DisallowConcurrentExecution]
public class DeleteRecordsJob : IJob
{
    private readonly IDirectionRepository _directionRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IIdeaRepository _ideaRepository;
    private readonly ILogger<DeleteRecordsJob> _logger;
    private readonly DeleteRecordsJobOptions _options;

    public DeleteRecordsJob(ILogger<DeleteRecordsJob> logger, IOptions<DeleteRecordsJobOptions> options,
        IDirectionRepository directionRepository, ICommentRepository commentRepository,
        ITaskRepository taskRepository, IIdeaRepository ideaRepository)
    {
        _directionRepository = directionRepository;
        _logger = logger;
        _options = options.Value;
        _commentRepository = commentRepository;
        _taskRepository = taskRepository;
        _ideaRepository = ideaRepository;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Deleting old soft deleted records");

        var deletedBeforeOrAt = DateTime.UtcNow.AddDays(-_options.DeleteAfterDays.GetValueOrDefault(30));

        await _directionRepository.HardDeleteSoftDeletedDirectionsWithChildren(deletedBeforeOrAt);
        await _taskRepository.HardDeleteSoftDeletedTasksWithChildren(deletedBeforeOrAt);
        await _ideaRepository.HardDeleteSoftDeletedIdeasWithChildren(deletedBeforeOrAt);
        await _commentRepository.HardDeleteSoftDeletedComments(deletedBeforeOrAt);
    }
}