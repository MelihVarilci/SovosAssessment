using Hangfire;
using SovosAssessment.WebAPI.Hangfire.Workers.InvoiceWorker;

namespace SovosAssessment.WebAPI.Hangfire
{
    public class HangfireService
    {
        public static void InitializeJobs()
        {
            // Her saat başı çalışacak şekilde worker'ı ayarlıyoruz.
            RecurringJob.AddOrUpdate<InvoiceWorker>(job => job.ExecuteAsync(0), "0 * * * *");
        }
    }
}
