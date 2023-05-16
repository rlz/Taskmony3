using Taskmony.Errors;
using Taskmony.Exceptions;

namespace Taskmony.Models.Tasks;

public class RecurrencePattern : Entity
{
    public Guid TaskId { get; private set; }

    public Task Task { get; private set; } = default!;

    public RepeatMode RepeatMode { get; private set; }

    public WeekDay? WeekDays { get; private set; }

    public int RepeatEvery { get; private set; }

    public DateTime RepeatUntil { get; private set; }

    // Required by EF Core
    private RecurrencePattern()
    {
    }

    public RecurrencePattern(RepeatMode? repeatMode, WeekDay? weekDays, int? repeatEvery, DateTime? repeatUntil)
    {
        Validate(repeatMode, weekDays, repeatEvery, repeatUntil);

        RepeatMode = repeatMode!.Value;
        WeekDays = weekDays;
        RepeatEvery = repeatEvery!.Value;
        RepeatUntil = repeatUntil!.Value;
    }

    public RecurrencePattern(Guid taskId, RepeatMode? repeatMode, WeekDay? weekDays, int? repeatEvery,
        DateTime? repeatUntil) : this(repeatMode, weekDays, repeatEvery, repeatUntil)
    {
        TaskId = taskId;
    }

    public void UpdateRepeatUntil(DateTime? repeatUntil)
    {
        if (repeatUntil == null)
        {
            throw new DomainException(ValidationErrors.RepeatUntilIsRequired);
        }

        RepeatUntil = repeatUntil.Value;
    }

    private void Validate(RepeatMode? repeatMode, WeekDay? weekDays, int? repeatEvery, DateTime? repeatUntil)
    {
        if (repeatMode == null)
        {
            throw new DomainException(ValidationErrors.RepeatModeIsRequired);
        }

        if (repeatEvery == null)
        {
            throw new DomainException(ValidationErrors.RepeatEveryIsRequired);
        }

        if (repeatEvery <= 0)
        {
            throw new DomainException(ValidationErrors.InvalidRepeatEvery);
        }

        if (repeatMode == RepeatMode.Week && weekDays == null)
        {
            throw new DomainException(ValidationErrors.WeekDaysAreRequired);
        }

        if (repeatUntil == null)
        {
            throw new DomainException(ValidationErrors.RepeatUntilIsRequired);
        }
    }
}