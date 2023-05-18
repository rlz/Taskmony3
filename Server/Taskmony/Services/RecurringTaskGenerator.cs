using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Tasks;
using Taskmony.Models.ValueObjects;
using Taskmony.Services.Abstract;
using Task = Taskmony.Models.Tasks.Task;

namespace Taskmony.Services;

public class RecurringTaskGenerator : IRecurringTaskGenerator
{
    public List<Task> CreateRecurringTaskInstances(Task task, RecurrencePattern pattern)
    {
        task.UpdateRecurrencePattern(pattern);
        task.UpdateGroupId(Guid.NewGuid());

        if (pattern.WeekDays is null || pattern.RepeatMode != RepeatMode.Week)
        {
            return GenerateTasks(task, pattern.RepeatUntil, pattern.RepeatMode, pattern.WeekDays, pattern.RepeatEvery);
        }

        return GenerateWeeklyTasks(task, pattern.RepeatUntil, pattern.RepeatMode, pattern.WeekDays.Value,
            pattern.RepeatEvery);
    }

    private List<Task> GenerateWeeklyTasks(Task task, DateTime repeatUntil, RepeatMode repeatMode,
        WeekDay weekDays, int repeatEvery)
    {
        var tasks = new List<Task>();
        var initialStartAt = task.StartAt!.Value;

        foreach (var day in Enum.GetValues(typeof(WeekDay)))
        {
            if (!weekDays.HasFlag((WeekDay) day))
            {
                continue;
            }

            var startAt = GetNextWeekday(initialStartAt, WeekDayToDayOfWeek((WeekDay) day));

            // Assuming that even if there is no fitting date on the current week, we start from the current week
            if (startAt > initialStartAt && startAt.DayOfWeek <= initialStartAt.DayOfWeek)
            {
                startAt = startAt.AddDays(7 * (repeatEvery - 1));
            }

            task = new Task(
                description: Description.From(task.Description!.Value),
                details: Details.From(task.Details?.Value),
                createdById: task.CreatedById,
                startAt: startAt,
                assignment: task.Assignment != null
                    ? new Assignment(task.Assignment.AssigneeId, task.Assignment.AssignedById)
                    : null,
                recurrencePattern: new RecurrencePattern(repeatMode, weekDays, repeatEvery, repeatUntil),
                groupId: task.GroupId,
                directionId: task.DirectionId,
                createdAt: task.CreatedAt);

            tasks.AddRange(GenerateTasks(task, repeatUntil, repeatMode, weekDays, repeatEvery));
        }

        return tasks;
    }

    private DayOfWeek WeekDayToDayOfWeek(WeekDay weekDay)
    {
        return weekDay switch
        {
            WeekDay.Monday => DayOfWeek.Monday,
            WeekDay.Tuesday => DayOfWeek.Tuesday,
            WeekDay.Wednesday => DayOfWeek.Wednesday,
            WeekDay.Thursday => DayOfWeek.Thursday,
            WeekDay.Friday => DayOfWeek.Friday,
            WeekDay.Saturday => DayOfWeek.Saturday,
            WeekDay.Sunday => DayOfWeek.Sunday,
            _ => throw new ArgumentOutOfRangeException(nameof(weekDay), weekDay, null)
        };
    }

    private static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
    {
        return start.AddDays(((int) day - (int) start.DayOfWeek + 7) % 7);
    }

    private List<Task> GenerateTasks(Task task, DateTime repeatUntil, RepeatMode repeatMode, WeekDay? weekDays,
        int repeatEvery)
    {
        var tasks = new List<Task>();
        var nextStartAt = task.StartAt!.Value;
        var initialStartAt = task.StartAt.Value;
        var repeatCount = 1;

        while (nextStartAt <= repeatUntil)
        {
            tasks.Add(new Task(
                description: Description.From(task.Description!.Value),
                details: Details.From(task.Details?.Value),
                createdById: task.CreatedById,
                startAt: nextStartAt,
                assignment: task.Assignment != null
                    ? new Assignment(task.Assignment.AssigneeId, task.Assignment.AssignedById)
                    : null,
                recurrencePattern: new RecurrencePattern(repeatMode, weekDays, repeatEvery, repeatUntil),
                groupId: task.GroupId,
                directionId: task.DirectionId,
                createdAt: task.CreatedAt));

            nextStartAt = GetNextDateTime(initialStartAt, repeatMode, repeatEvery, repeatCount++);
        }

        return tasks;
    }

    private static DateTime GetNextDateTime(DateTime initial, RepeatMode repeatMode, int repeatEvery, int repeatCount)
    {
        return repeatMode switch
        {
            RepeatMode.Day => initial.AddDays(repeatEvery * repeatCount),
            RepeatMode.Week => initial.AddDays(7 * repeatEvery * repeatCount),
            RepeatMode.Month => initial.AddMonths(repeatEvery * repeatCount),
            RepeatMode.Year => initial.AddYears(repeatEvery * repeatCount),
            _ => throw new DomainException(ValidationErrors.InvalidRepeatMode)
        };
    }
}