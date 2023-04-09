using SovosAssessment.Domain.Entities.Common.Concrete;

namespace SovosAssessment.Infrastructure.Persistence.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IInvoiceRepository Invoice { get; }
        IInvoiceLineRepository InvoiceLine { get; }

        IGenericRepository<T> GetRepository<T>() where T : BaseEntity;
        void SaveChanges();
    }
}

