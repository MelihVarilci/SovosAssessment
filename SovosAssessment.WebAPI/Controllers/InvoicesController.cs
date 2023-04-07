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
        public IActionResult UploadJson([FromBody] JObject jsonData)
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
                _unitOfWork.Invoice.AddInvoice(invoice);
                _unitOfWork.SaveChanges();

                // InvoiceLine verilerini kaydetme işlemini yapıyoruz
                foreach (var invoiceLineData in invoiceData.InvoiceLine)
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
                    _unitOfWork.InvoiceLine.AddInvoiceLine(invoiceLine);
                    _unitOfWork.SaveChanges();
                }

                Console.WriteLine("Kayıt başarıyla eklendi.");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Kayıt eklenirken hata oluştu: " + ex.Message);
                return BadRequest(ex);
            }
        }
    }
}
