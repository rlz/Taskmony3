namespace Taskmony.Schedulers;

public class DeleteRecordsJobOptions
{
    public string? CronExpression { get; set; }

    public int? DeleteAfterDays { get; set; }

    public bool? Enabled { get; set; }
}