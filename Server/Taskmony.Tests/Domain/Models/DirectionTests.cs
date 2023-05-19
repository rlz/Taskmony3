using Taskmony.Exceptions;
using Taskmony.Models.Directions;
using Taskmony.Models.ValueObjects;

namespace Taskmony.Tests.Domain.Models;

public class DirectionTests
{
    [Fact]
    public void CreateDirection()
    {
        var userId = Guid.NewGuid();
        var direction = new Direction(userId, DirectionName.From("name"), Details.From("details"));

        Assert.Equal(DirectionName.From("name"), direction.Name);
        Assert.Equal(Details.From("details"), direction.Details);
        Assert.Equal(userId, direction.CreatedById);
        Assert.Null(direction.DeletedAt);
        Assert.NotNull(direction.CreatedAt);
    }

    [Fact]
    public void UpdateName()
    {
        var direction = GetValidDirection();
        var name = DirectionName.From("new name");

        direction.UpdateName(name);

        Assert.Equal(name, direction.Name);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("      ")]
    [InlineData(null)]
    public void UpdateName_ThrowsWhenInvalid(string name)
    {
        var direction = GetValidDirection();
        
        Assert.Throws<DomainException>(() => direction.UpdateName(DirectionName.From(name)));
        Assert.Throws<DomainException>(() => direction.UpdateName(DirectionName.From(new string('a', 256))));
    }

    [Fact]
    public void UpdateDetails()
    {
        var direction = GetValidDirection();
        var details = Details.From("new details");

        direction.UpdateDetails(details);

        Assert.Equal(details, direction.Details);
    }
    
    [Fact]
    public void UpdateDetails_ThrowsWhenInvalid()
    {
        var direction = GetValidDirection();
        
        Assert.Throws<DomainException>(() => direction.UpdateDetails(Details.From(new string('a', 100000))));
    }

    [Fact]
    public void UpdateDeletedAt()
    {
        var direction = GetValidDirection();
        var deletedAt = DeletedAt.From(DateTime.UtcNow);

        direction.UpdateDeletedAt(deletedAt);

        Assert.Equal(deletedAt, direction.DeletedAt);
    }
    
    [Fact]
    public void UpdateDeletedAt_ThrowsWhenAlreadyDeleted()
    {
        var direction = GetValidDirection();
        var deletedAt = DeletedAt.From(DateTime.UtcNow);
        
        direction.UpdateDeletedAt(deletedAt);
        
        Assert.Throws<DomainException>(() => direction.UpdateDeletedAt(deletedAt));
    }

    [Fact]
    public void UpdateDeletedAt_Undelete()
    {
        var direction = GetValidDirection();
        var deletedAt = DeletedAt.From(DateTime.UtcNow);
        
        direction.UpdateDeletedAt(deletedAt);
        Assert.Equal(deletedAt, direction.DeletedAt);

        direction.UpdateDeletedAt(null);
        Assert.Null(direction.DeletedAt);
    }

    [Fact]
    public void UpdateDeletedDirection_Throws()
    {
        var direction = GetValidDirection();
        
        direction.UpdateDeletedAt(DeletedAt.From(DateTime.UtcNow));
        
        Assert.Throws<DomainException>(() => direction.UpdateName(DirectionName.From("new name")));
        Assert.Throws<DomainException>(() => direction.UpdateDetails(Details.From("new details")));
        Assert.Throws<DomainException>(() => direction.UpdateDeletedAt(DeletedAt.From(DateTime.UtcNow)));
    }

    private Direction GetValidDirection()
    {
        return new Direction(Guid.NewGuid(), DirectionName.From("name"), Details.From("details"));
    }
}