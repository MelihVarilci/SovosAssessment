using SovosAssessment.Application.DTOs;
using SovosAssessment.Application.Result;
using SovosAssessment.Domain.Entities;

namespace SovosAssessment.Infrastructure.Persistence.Repositories
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        Task<IDataResult<List<Invoice>>> GetAllInvoices();

        Task<IDataResult<Invoice?>> GetInvoiceByExternalInvoiceId(string externalInvoiceId);

        Task<IDataResult<Invoice>> AddInvoice(InvoiceData invoiceData);

        IResult UpdateInvoice(Invoice invoice);

        Task<IResult> DeleteInvoice(int id);
    }
}
