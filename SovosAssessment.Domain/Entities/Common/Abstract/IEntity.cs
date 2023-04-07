namespace SovosAssessment.Domain.Entities.Common.Abstract;

public interface IEntity : IEntity<int>
{

}

public interface IEntity<TPrimaryKey>
{
    public TPrimaryKey Id { get; set; }

    public DateTime CreatedAt { get; set; }
}