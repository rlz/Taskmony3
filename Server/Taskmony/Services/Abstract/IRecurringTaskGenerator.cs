using Taskmony.Models.Tasks;
using Task = Taskmony.Models.Tasks.Task;

namespace Taskmony.Services.Abstract;

public interface IRecurringTaskGenerator
{
    public List<Task> CreateRecurringTaskInstances(Task task, RecurrencePattern pattern);
}