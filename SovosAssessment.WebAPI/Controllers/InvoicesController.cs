using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using SovosAssessment.Application.Abstractions.Services;
using SovosAssessment.Application.DTOs;
using SovosAssessment.Application.Result;
using SovosAssessment.Domain.Entities;
using SovosAssessment.Infrastructure.Persistence.Repositories;

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
                Log.Information("Throw Exeption: {Ex}", ex);
                return BadRequest(ex);
            }
        }

        [HttpGet("getInvoiceByInvoiceId")]
        public async Task<IActionResult> GetInvoiceByExternalInvoiceId(string invoiceId)
        {
            try
            {
                var result = await _unitOfWork.Invoice.GetInvoiceByExternalInvoiceId(invoiceId);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                Log.Information("Throw Exeption: {Ex}", ex);
                return BadRequest(ex);
            }
        }

        [HttpPost("uploadJson")]
        public async Task<IActionResult> UploadJson([FromBody] JObject jsonData)
        {
            try
            {
                // JSON belgesini işlemek için gelen json nesnesini InvoiceData nesnesine deserialize ediyoruz
                var invoiceData = JsonConvert.DeserializeObject<InvoiceData>(jsonData.ToString());

                var invoiceResult = await _unitOfWork.Invoice.AddInvoice(invoiceData);

                if (invoiceResult.Success && invoiceResult.Data != null)
                {
                    // InvoiceLine verilerini kaydetme işlemini yapıyoruz
                    foreach (var invoiceLineData in invoiceData.InvoiceLine)
                    {
                        var invoiceLine = new InvoiceLine
                        {
                            Name = invoiceLineData.Name,
                            Quantity = invoiceLineData.Quantity,
                            UnitCode = invoiceLineData.UnitCode,
                            UnitPrice = invoiceLineData.UnitPrice,
                            InvoiceFk = invoiceResult.Data
                        };

                        // InvoiceLine nesnesini veritabanına kaydet
                        await _unitOfWork.InvoiceLine.AddInvoiceLine(invoiceLine);
                    }

                    // Veritabanına toplu kayıt için yazmış olduğumuz unitoOfWork saveChange methodunu kullanıyoruz
                    _unitOfWork.SaveChanges();

                    return Ok(invoiceResult);
                }

                return BadRequest(invoiceResult);
            }
            catch (Exception ex)
            {
                Log.Information("Throw Exeption: {Ex}", ex);
                return BadRequest(ex);
            }
        }

        [Produces("application/json")]
        [HttpPost("uploadJsonWithFile")]
        public async Task<IActionResult> UploadJsonWithFile(IFormFile file)
        {
            try
            {
                // Belge tipi json olanlar için işlemlere devam ediyoruz
                // eğer json harici bir belge gönderilmişse adet bilgisiyle kullanıcı bilgilendirme mesajı hazırlıyoruz
                if (file != null && file.ContentType == "application/json")
                {
                    using (var streamReader = new StreamReader(file.OpenReadStream()))
                    {
                        // Yüklenecek json belgesinin içerisindeki bilgileri alıyoruz
                        var json = streamReader.ReadToEnd();

                        // JSON belgesini işlemek için gelen json nesnesini InvoiceData nesnesine deserialize ediyoruz
                        var invoiceData = JsonConvert.DeserializeObject<InvoiceData>(json);

                        var invoiceResult = await _unitOfWork.Invoice.AddInvoice(invoiceData);

                        if (invoiceResult.Success && invoiceResult.Data != null)
                        {

                            // InvoiceLine verilerini kaydetme işlemini yapıyoruz
                            foreach (var invoiceLineData in invoiceData.InvoiceLine)
                            {
                                var invoiceLine = new InvoiceLine
                                {
                                    Name = invoiceLineData.Name,
                                    Quantity = invoiceLineData.Quantity,
                                    UnitCode = invoiceLineData.UnitCode,
                                    UnitPrice = invoiceLineData.UnitPrice,
                                    InvoiceFk = invoiceResult.Data
                                };

                                // InvoiceLine nesnesini veritabanına kaydet
                                await _unitOfWork.InvoiceLine.AddInvoiceLine(invoiceLine);
                            }

                            // Veritabanına toplu kayıt için yazmış olduğumuz unitoOfWork saveChange methodunu kullanıyoruz
                            _unitOfWork.SaveChanges();

                            return Ok(invoiceResult);
                        }

                        return BadRequest(invoiceResult);
                    }
                }
                else
                {
                    return BadRequest(new SuccessDataResult<Invoice>(null, "json formatında olmayan dosyalar geçersizdir. Formata uymayan dosya tespit edildi."));
                }

            }
            catch (Exception ex)
            {
                Log.Information("Throw Exeption: {Ex}", ex);
                return BadRequest(ex);
            }
        }
    }
}

