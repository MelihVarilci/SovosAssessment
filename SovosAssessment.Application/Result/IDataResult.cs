namespace SovosAssessment.Application.Result
{
    public interface IDataResult<T> : IResult
    {
        T Data { get; }
    }
}
