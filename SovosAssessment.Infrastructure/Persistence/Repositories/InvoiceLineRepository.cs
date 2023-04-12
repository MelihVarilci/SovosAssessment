using Microsoft.EntityFrameworkCore;
using SovosAssessment.Application.Result;
using SovosAssessment.Domain.Entities;
using SovosAssessment.Infrastructure.Persistence.Context;
using SovosAssessment.Infrastructure.Persistence.Repositories;

namespace SovosAssessment.Application.Abstractions.Services
{
    public class InvoiceLineRepository : GenericRepository<InvoiceLine>, IInvoiceLineRepository
    {
        public InvoiceLineRepository(SovosAssessmentDbContext context) : base(context)
        {
        }

        public async Task<IDataResult<List<InvoiceLine>>> GetAllInvoiceLines()
        {
            var invoiceLines = await GetAll()
                .OrderBy(x => x.Id)
                .ToListAsync();

            return new SuccessDataResult<List<InvoiceLine>>(invoiceLines);
        }

        public async Task<IDataResult<InvoiceLine>> GetInvoiceLineById(int id)
        {
            var invoiceLine = await GetByIdAsync(id);

            return new SuccessDataResult<InvoiceLine>(invoiceLine);
        }

        public async Task<IDataResult<InvoiceLine>> AddInvoiceLine(InvoiceLine invoiceLine)
        {
            var result = await InsertAsync(invoiceLine);

            return new SuccessDataResult<InvoiceLine>(result);
        }

        public IResult UpdateInvoiceLine(InvoiceLine invoiceLine)
        {
            Update(invoiceLine);

            return new SuccessResult();
        }

        public async Task<IResult> DeleteInvoiceLine(int id)
        {
            await DeleteAsync(id);

            return new SuccessResult();
        }
    }
}
