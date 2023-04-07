using SovosAssessment.Domain.Entities;

namespace SovosAssessment.Infrastructure.Persistence.Repositories
{
    public interface IInvoiceLineRepository : IGenericRepository<InvoiceLine>
    {
        Task<List<InvoiceLine>> GetAllInvoiceLines();

        Task<InvoiceLine> GetInvoiceLineById(int id);

        Task<InvoiceLine> AddInvoiceLine(InvoiceLine invoiceLine);

        void UpdateInvoiceLine(InvoiceLine invoiceLine);

        Task DeleteInvoiceLine(int id);
    }
}
