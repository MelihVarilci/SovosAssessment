using SovosAssessment.Application.Result;
using SovosAssessment.Domain.Entities;

namespace SovosAssessment.Infrastructure.Persistence.Repositories
{
    public interface IInvoiceLineRepository : IGenericRepository<InvoiceLine>
    {
        Task<IDataResult<List<InvoiceLine>>> GetAllInvoiceLines();

        Task<IDataResult<InvoiceLine>> GetInvoiceLineById(int id);

        Task<IDataResult<InvoiceLine>> AddInvoiceLine(InvoiceLine invoiceLine);

        IResult UpdateInvoiceLine(InvoiceLine invoiceLine);

        Task<IResult> DeleteInvoiceLine(int id);
    }
}
