using SovosAssessment.Domain.Entities;

namespace SovosAssessment.Infrastructure.Persistence.Repositories
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        Task<List<Invoice>> GetAllInvoices();

        Task<Invoice> GetInvoiceByExternalInvoiceId(string externalInvoiceId);

        Task<Invoice> AddInvoice(Invoice invoice);

        void UpdateInvoice(Invoice invoice);

        Task DeleteInvoice(int id);
    }
}
