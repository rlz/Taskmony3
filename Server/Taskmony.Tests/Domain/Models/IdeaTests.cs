using Taskmony.Exceptions;
using Taskmony.Models.Ideas;
using Taskmony.Models.ValueObjects;

namespace Taskmony.Tests.Domain.Models;

public class IdeaTests
{
    [Fact]
    public void CreateIdea()
    {
        var userId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var idea = new Idea(
            description: Description.From("description"),
            details: Details.From("details"),
            createdById: userId,
            generation: Generation.Hot,
            directionId: directionId);

        Assert.Equal(userId, idea.CreatedById);
        Assert.Equal(Generation.Hot, idea.Generation);
        Assert.Equal(Description.From("description"), idea.Description);
        Assert.Equal(Details.From("details"), idea.Details);
        Assert.Null(idea.DeletedAt);
        Assert.Null(idea.ReviewedAt);
        Assert.NotNull(idea.CreatedAt);
        Assert.NotNull(idea.DirectionId);
    }

    [Fact]
    public void UpdateDescription()
    {
        var idea = GetValidIdea();
        var description = Description.From("new description");

        idea.UpdateDescription(description);

        Assert.Equal(description, idea.Description);
    }

    [Fact]
    public void UpdateDetails()
    {
        var idea = GetValidIdea();
        var details = Details.From("new details");

        idea.UpdateDetails(details);

        Assert.Equal(details, idea.Details);
    }

    [Fact]
    public void UpdateGeneration()
    {
        var idea = GetValidIdea();
        var generation = Generation.Later;

        idea.UpdateGeneration(generation);

        Assert.Equal(generation, idea.Generation);
    }
    
    [Fact]
    public void UpdateReviewedAt()
    {
        var idea = GetValidIdea();
        var reviewedAt = ReviewedAt.From(DateTime.UtcNow);

        idea.UpdateReviewedAt(reviewedAt);

        Assert.Equal(reviewedAt, idea.ReviewedAt);
    }

    [Fact]
    public void UpdateReviewedAt_ThrowsWhenInvalid()
    {
        Assert.Throws<DomainException>(() => ReviewedAt.From(DateTime.UtcNow.AddDays(1)));
    }
    
    [Fact]
    public void UpdateDeletedAt()
    {
        var idea = GetValidIdea();
        var deletedAt = DeletedAt.From(DateTime.UtcNow);

        idea.UpdateDeletedAt(deletedAt);

        Assert.Equal(deletedAt, idea.DeletedAt);
    }

    [Fact]
    public void UpdateDeletedAt_Undelete()
    {
        var idea = GetValidIdea();
        var deletedAt = DeletedAt.From(DateTime.UtcNow);

        idea.UpdateDeletedAt(deletedAt);
        Assert.Equal(deletedAt, idea.DeletedAt);
        
        idea.UpdateDeletedAt(null);
        Assert.Null(idea.DeletedAt);
    }
    
    [Fact]
    public void UpdateDeletedAt_ThrowsWhenAlreadyDeleted()
    {
        var idea = GetValidIdea();
        var deletedAt = DeletedAt.From(DateTime.UtcNow);

        idea.UpdateDeletedAt(deletedAt);

        Assert.Throws<DomainException>(() => idea.UpdateDeletedAt(deletedAt));
    }
    
    [Fact]
    public void UpdateDirectionId()
    {
        var idea = GetValidIdea();
        var directionId = Guid.NewGuid();

        idea.UpdateDirectionId(directionId);

        Assert.Equal(directionId, idea.DirectionId);
    }

    [Fact]
    public void UpdateDeletedIdea_Throws()
    {
        var idea = GetValidIdea();
        
        idea.UpdateDeletedAt(DeletedAt.From(DateTime.UtcNow));
        
        Assert.Throws<DomainException>(() => idea.UpdateDescription(Description.From("new description")));
        Assert.Throws<DomainException>(() => idea.UpdateDetails(Details.From("new details")));
        Assert.Throws<DomainException>(() => idea.UpdateGeneration(Generation.Later));
        Assert.Throws<DomainException>(() => idea.UpdateReviewedAt(ReviewedAt.From(DateTime.UtcNow)));
        Assert.Throws<DomainException>(() => idea.UpdateDirectionId(Guid.NewGuid()));
    }

    private Idea GetValidIdea()
    {
        return new Idea(
            description: Description.From("description"),
            details: Details.From("details"),
            createdById: Guid.NewGuid(),
            generation: Generation.Hot,
            directionId: null);
    }
}