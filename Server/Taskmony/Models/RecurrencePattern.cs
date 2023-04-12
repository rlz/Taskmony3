using Taskmony.Models.Enums;

namespace Taskmony.Models;

public class RecurrencePattern : Entity
{
    public Guid TaskId { get; set; }

    public Task Task { get; set; } = default!;

    public RepeatMode RepeatMode { get; set; }

    public WeekDay? WeekDays { get; set; }

    public int RepeatEvery { get; set; }

    public DateTime RepeatUntil { get; set; }
}