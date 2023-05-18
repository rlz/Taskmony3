using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Spi;

namespace Taskmony.Schedulers;

public class QuartzScheduler : IHostedService
{
    private IScheduler? _scheduler;
    private readonly DeleteRecordsJobOptions _options;
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IJobFactory _jobFactory;

    public QuartzScheduler(IOptions<DeleteRecordsJobOptions> options, ISchedulerFactory schedulerFactory,
        IJobFactory jobFactory)
    {
        _options = options.Value;
        _schedulerFactory = schedulerFactory;
        _jobFactory = jobFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled.GetValueOrDefault(false))
        {
            return;
        }
        
        _scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        _scheduler.JobFactory = _jobFactory;
        
        await _scheduler.Start(cancellationToken);

        var job = JobBuilder.Create<DeleteRecordsJob>()
            .WithIdentity("deleteRecordsJob", "group1")
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity("deleteRecordsTrigger", "group1")
            .WithCronSchedule(_options.CronExpression ?? "0 0 0 * * ?")
            .StartNow()
            .ForJob(job)
            .Build();

        await _scheduler.ScheduleJob(job, trigger, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_scheduler != null)
        {
            await _scheduler.Shutdown(cancellationToken);
        }
    }
}