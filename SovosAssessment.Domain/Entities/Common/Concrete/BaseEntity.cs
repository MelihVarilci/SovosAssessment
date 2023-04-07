using SovosAssessment.Domain.Entities.Common.Abstract;

namespace SovosAssessment.Domain.Entities.Common.Concrete;

public class BaseEntity<TPrimaryKey> : IEntity<TPrimaryKey>
{
    public TPrimaryKey Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public abstract class BaseEntity : BaseEntity<int>, IEntity
{

}