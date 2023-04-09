using Microsoft.EntityFrameworkCore;
using SovosAssessment.Domain.Entities;
using SovosAssessment.Infrastructure.Persistence.Context;
using SovosAssessment.Infrastructure.Persistence.Repositories;

namespace SovosAssessment.Application.Abstractions.Services
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(SovosAssessmentDbContext context) : base(context)
        {
        }

        public async Task<List<Invoice>> GetAllInvoices()
        {
            var invoices = await GetAll()
                .Include(x => x.InvoiceLines)
                .OrderBy(x => x.Id)
                .ToListAsync();

            return invoices;
        }

        public async Task<Invoice> GetInvoiceByExternalInvoiceId(string externalInvoiceId)
        {
            var invoice = await GetAll()
                .Include(x => x.InvoiceLines)
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync(x => x.ExternalInvoiceId == externalInvoiceId);

            return invoice;
        }

        public async Task<Invoice> AddInvoice(Invoice invoice)
        {
            var result = await InsertAsync(invoice);

            return result;
        }

        public void UpdateInvoice(Invoice invoice)
        {
            Update(invoice);
        }

        public async Task DeleteInvoice(int id)
        {
            await DeleteAsync(id);
        }
    }
}
