using Microsoft.EntityFrameworkCore;
using SovosAssessment.Application.DTOs;
using SovosAssessment.Application.Result;
using SovosAssessment.Domain.Entities;
using SovosAssessment.Infrastructure.Persistence.Context;
using SovosAssessment.Infrastructure.Persistence.Repositories;

namespace SovosAssessment.Application.Abstractions.Services
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        //private readonly IInvoiceLineRepository _invoiceLineRepository;


        public InvoiceRepository(SovosAssessmentDbContext context) : base(context)
        {
        }

        public async Task<IDataResult<List<Invoice>>> GetAllInvoices()
        {
            var invoices = await GetAll()
                .Include(x => x.InvoiceLines)
                .OrderBy(x => x.Id)
                .ToListAsync();

            return new SuccessDataResult<List<Invoice>>(invoices, invoices.Count > 0 ? "Tüm faturlar listelendi." : "Hali hazırda kayıtlı bir fatura bulunamadı.");
        }

        public async Task<IDataResult<Invoice?>> GetInvoiceByExternalInvoiceId(string externalInvoiceId)
        {
            var invoice = await GetAll()
                .Include(x => x.InvoiceLines)
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync(x => x.ExternalInvoiceId == externalInvoiceId);

            return new SuccessDataResult<Invoice?>(invoice, invoice != null ? "İlgili fatura listelendi." : "İlgili fatura bulunamadı.");
        }

        public async Task<IDataResult<Invoice?>> AddInvoice(InvoiceData invoiceData)
        {
            var invoice = new Invoice
            {
                ExternalInvoiceId = invoiceData.InvoiceHeader.InvoiceId,
                SenderTitle = invoiceData.InvoiceHeader.SenderTitle,
                ReceiverTitle = invoiceData.InvoiceHeader.ReceiverTitle,
                Date = invoiceData.InvoiceHeader.Date
            };

            // Kayıt edilecek invoice id'si önceden kayıt edilmiş mi?
            var alreadyIsHave = await GetInvoiceByExternalInvoiceId(invoice.ExternalInvoiceId);

            if (alreadyIsHave.Data != null)
            {
                return new SuccessDataResult<Invoice?>(null, "Fatura hali hazırda mevcut.");
            }

            // Invoice nesnesini veritabanına kaydet
            var result = await InsertAsync(invoice);

            return new SuccessDataResult<Invoice?>(result, "Fatura başarılı bir şekilde oluşturuldu.");
        }

        public IResult UpdateInvoice(Invoice invoice)
        {
            Update(invoice);

            return new SuccessResult();
        }

        public async Task<IResult> DeleteInvoice(int id)
        {
            await DeleteAsync(id);

            return new SuccessResult();
        }
    }
}
