using SovosAssessment.Domain.Entities.Common.Abstract;

namespace SovosAssessment.Domain.Entities.Common.Concrete;

public class Entity : Entity<int>, IEntity
{
}

public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
{
    public virtual TPrimaryKey Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

