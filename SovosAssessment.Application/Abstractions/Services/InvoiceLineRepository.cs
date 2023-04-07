using Microsoft.EntityFrameworkCore;
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

        public async Task<List<InvoiceLine>> GetAllInvoiceLines()
        {
            var invoiceLines = await GetAll()
                .OrderBy(x => x.Id)
                .ToListAsync();

            return invoiceLines;
        }

        public async Task<InvoiceLine> GetInvoiceLineById(int id)
        {
            var invoiceLine = await GetByIdAsync(id);

            return invoiceLine;
        }

        public async Task<InvoiceLine> AddInvoiceLine(InvoiceLine invoiceLine)
        {
            var result = await InsertAsync(invoiceLine);

            return result;
        }

        public void UpdateInvoiceLine(InvoiceLine invoiceLine)
        {
            Update(invoiceLine);
        }

        public async Task DeleteInvoiceLine(int id)
        {
            await DeleteAsync(id);
        }
    }
}
