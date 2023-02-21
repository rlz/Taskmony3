using Taskmony.Models;
using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;
using Task = Taskmony.Models.Task;

namespace Taskmony.Services.Abstract;

public interface INotificationService
{
    Task<IEnumerable<Notification>> GetNotificationsByNotifiableIdsAsync(NotifiableType type, Guid[] ids, 
        DateTime? start, DateTime? end, Guid currentUserId);
    
    Task<bool> NotifyTaskAssigneeUpdatedAsync(Task task, Guid? oldAssigneeId, Guid modifiedById,
        DateTime? modifiedAt);

    /// <summary>
    /// Creates notifications about assigned task and entity added to the direction if 
    /// direction id is not null and there are any members other than owner
    /// </summary>
    /// <param name="entity">Added task or idea</param>
    /// <param name="createdAt">Date and time of creation</param>
    /// <param name="createdById">Creator</param>
    /// <returns>Whether any notifications were created</returns>
    Task<bool> NotifyDirectionEntityAddedAsync(DirectionEntity entity, DateTime? createdAt,
        Guid createdById);

    /// <summary>
    /// Creates notifications about updated field if direction id is not null and there are any 
    /// members other than modifier
    /// </summary>
    /// <param name="entity">Updated task or idea</param>
    /// <param name="field">Name of the updated field</param>
    /// <param name="oldValue">Old value of the updated field</param>
    /// <param name="newValue">New value of the updated field</param>
    /// <param name="modifiedById">User who updated entity</param>
    /// <returns>Whether any notifications were created</returns>
    Task<bool> NotifyDirectionEntityUpdatedAsync(DirectionEntity entity, string field, string? oldValue,
        string? newValue, Guid modifiedById);

    /// <summary>
    /// Created notifications about entity removed from direction if deleted at is set or
    /// entity updated if deleted at is null
    /// </summary>
    /// <param name="entity">Upddated task or idea</param>
    /// <param name="oldDeletedAt">Old value of deleted at</param>
    /// <param name="newDeletedAt">New value of deleted at</param>
    /// <param name="modifiedById">User who updated entity</param>
    /// <returns>Whether any notifications were created></returns>
    Task<bool> NotifyDirectionEntityDeletedAtUpdatedAsync(DirectionEntity entity, DateTime? oldDeletedAt,
        DateTime? newDeletedAt, Guid modifiedById);

    /// <summary>
    /// Created notifications about entity added to the direction if direction is not null and
    /// entity removed if old direction is not null
    /// </summary>
    /// <param name="entity">Updated task or idea</param>
    /// <param name="oldDirectionId">Old direction id</param>
    /// <param name="modifiedById">User who updated entity</param>
    /// <param name="modifiedAt">Date and time of update</param>
    /// <returns>Whether any notifications were created</returns>
    Task<bool> NotifyDirectionEntityMovedAsync(DirectionEntity entity, Guid? oldDirectionId,
        Guid modifiedById, DateTime? modifiedAt);

    Task<bool> NotifyCommentAddedAsync(DirectionEntity entity, Guid commentId, Guid createdById,
        DateTime? createdAt);

    Task<bool> NotifyDirectionUpdatedAsync(Guid directionId, string field, string? oldValue,
        string? newValue, Guid modifiedById);

    Task<bool> NotifyDirectionMemberAddedAsync(Guid directionId, Guid memberId, DateTime? modifiedAt,
        Guid modifiedById);

    Task<bool> NotifyDirectionMemberRemovedAsync(Guid directionId, Guid memberId, DateTime? modifiedAt,
       Guid modifiedById);
}