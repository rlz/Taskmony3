// using Taskmony.Models.Ideas;
// using Taskmony.Models.ValueObjects;
//
// namespace Taskmony.Tests.Fixtures;
//
// public static class IdeaFixture
// {
//     public static IEnumerable<Idea> GetIdeas(Guid userId)
//     {
//         var otherUserId = Guid.NewGuid();
//         var privateDirectionCreatedByUser = DirectionFixture.GetPrivateUserDirection(userId);
//         var privateDirectionCreatedByOtherUser = DirectionFixture.GetPrivateUserDirection(otherUserId);
//         var publicDirectionCreatedByUser = DirectionFixture.GetPublicDirectionCreatedByUser(userId, otherUserId);
//         var publicDirectionCreatedByOtherUser = DirectionFixture.GetPublicDirectionCreatedByUser(otherUserId, userId);
//
//         return new List<Idea>
//         {
//             new(Guid.NewGuid(),
//                 Description.From("Private idea with no direction"),
//                 null,
//                 otherUserId,
//                 Generation.Hot),
//             new(Guid.NewGuid(),
//                 Description.From("Private user idea with no direction"),
//                 Details.From(null),
//                 userId,
//                 Generation.Hot,
//                 null),
//             new(Guid.NewGuid(),
//                 Description.From("Private user idea"),
//                 Details.From(null),
//                 userId,
//                 Generation.TooGoodToDelete,
//                 privateDirectionCreatedByUser.Id,
//                 privateDirectionCreatedByUser),
//             new(Guid.NewGuid(),
//                 Description.From("Private idea"),
//                 Details.From(null),
//                 otherUserId,
//                 Generation.Hot,
//                 privateDirectionCreatedByOtherUser.Id,
//                 privateDirectionCreatedByOtherUser),
//             new(Guid.NewGuid(),
//                 Description.From("Public user idea created by user"),
//                 Details.From(null),
//                 userId,
//                 Generation.Later,
//                 publicDirectionCreatedByUser.Id,
//                 publicDirectionCreatedByUser),
//             new(Guid.NewGuid(),
//                 Description.From("Public user idea created by other user"),
//                 Details.From(null),
//                 otherUserId,
//                 Generation.Later,
//                 publicDirectionCreatedByUser.Id,
//                 publicDirectionCreatedByUser),
//             new(Guid.NewGuid(),
//                 Description.From("Public user idea created by user"),
//                 Details.From(null),
//                 userId,
//                 Generation.Later,
//                 publicDirectionCreatedByOtherUser.Id,
//                 publicDirectionCreatedByOtherUser),
//             new(Guid.NewGuid(),
//                 Description.From("Public user idea created by other user"),
//                 Details.From(null),
//                 otherUserId,
//                 Generation.Later,
//                 publicDirectionCreatedByOtherUser.Id,
//                 publicDirectionCreatedByOtherUser)
//         };
//     }
// }