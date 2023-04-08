namespace SovosAssessment.WebAPI.BackgroundJobs
{
    public abstract class BackgroundJob<TArgs> : BackgroundJobBase<TArgs>, IBackgroundJob<TArgs>
    {
        public abstract Task ExecuteAsync(TArgs args);
    }
}