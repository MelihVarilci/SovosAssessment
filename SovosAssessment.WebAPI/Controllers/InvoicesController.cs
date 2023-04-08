using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SovosAssessment.Domain.Entities;
using SovosAssessment.Infrastructure.Persistence.Repositories;
using SovosAssessment.WebAPI.DTOs;

namespace SovosAssessment.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoicesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("getAllInvoices")]
        public async Task<IActionResult> GetAllInvoices()
        {
            try
            {
                var result = await _unitOfWork.Invoice.GetAllInvoices();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("getInvoiceByInvoiceId")]
        public async Task<IActionResult> GetInvoiceByExternalInvoiceId(string invoiceId)
        {
            try
            {
                var result = await _unitOfWork.Invoice.GetInvoiceByExternalInvoiceId(invoiceId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("uploadJson")]
        public async Task<IActionResult> UploadJson([FromBody] JObject jsonData)
        {
            try
            {
                // JSON belgesini işlemek için gelen json nesnesini önceden oluşturmuş olduğumuz InvoiceData nesnesine deserialize ediyoruz
                var invoiceData = JsonConvert.DeserializeObject<InvoiceData>(jsonData.ToString());

                var invoice = new Invoice
                {
                    ExternalInvoiceId = invoiceData.InvoiceHeader.InvoiceId,
                    SenderTitle = invoiceData.InvoiceHeader.SenderTitle,
                    ReceiverTitle = invoiceData.InvoiceHeader.ReceiverTitle,
                    Date = invoiceData.InvoiceHeader.Date
                };

                // Invoice nesnesini veritabanına kaydet
                await _unitOfWork.Invoice.AddInvoice(invoice);

                // InvoiceLine verilerini kaydetme işlemini yapıyoruz
                foreach (var invoiceLineData in invoiceData.InvoiceLines)
                {
                    var invoiceLine = new InvoiceLine
                    {
                        Name = invoiceLineData.Name,
                        Quantity = invoiceLineData.Quantity,
                        UnitCode = invoiceLineData.UnitCode,
                        UnitPrice = invoiceLineData.UnitPrice,
                        InvoiceFk = invoice
                    };

                    // InvoiceLine nesnesini veritabanına kaydet
                    await _unitOfWork.InvoiceLine.AddInvoiceLine(invoiceLine);
                }

                // Veritabanına toplu kayıt için yazmış olduğumuz unitoOfWork saveChange methodunu kullanıyoruz
                _unitOfWork.SaveChanges();

                return Ok("Invoice has been created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
