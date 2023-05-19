// using Taskmony.Models;
// using Taskmony.Models.Directions;
// using Taskmony.Models.Users;
// using Taskmony.Models.ValueObjects;
//
// namespace Taskmony.Tests.Fixtures;
//
// public static class DirectionFixture
// {
//     public static Direction GetDirectionWithoutMembers()
//     {
//         return new Direction(Guid.NewGuid(), Guid.NewGuid(), DirectionName.From("Direction"), null, null);
//     }
//
//     public static Direction GetPrivateUserDirection(Guid userId)
//     {
//         return new Direction(
//             Guid.NewGuid(),
//             userId,
//             DirectionName.From("Private user direction"),
//             null,
//             null);
//     }
//
//     public static Direction GetPublicDirectionCreatedByUser(Guid userId, Guid otherUserId)
//     {
//         return new Direction(
//             Guid.NewGuid(),
//             userId,
//             DirectionName.From("Public user direction"),
//             null,
//             new List<User>
//             {
//                 new(otherUserId,
//                     Login.From("login"),
//                     DisplayName.From("display name"),
//                     Email.From("email"),
//                     null),
//                 new(userId,
//                     Login.From("login"),
//                     DisplayName.From("display name"),
//                     Email.From("email"),
//                     null),
//             });
//     }
// }