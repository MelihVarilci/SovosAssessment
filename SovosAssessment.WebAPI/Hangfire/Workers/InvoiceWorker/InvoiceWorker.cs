using Hangfire;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Serilog;
using SovosAssessment.Application.Abstractions.Services;
using SovosAssessment.Application.DTOs;
using SovosAssessment.Domain.Entities;
using SovosAssessment.Infrastructure.Persistence.Repositories;
using SovosAssessment.WebAPI.BackgroundJobs;

namespace SovosAssessment.WebAPI.Hangfire.Workers.InvoiceWorker
{
    public class InvoiceWorker : BackgroundJob<int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly MailService _mailService;
        private readonly IConfiguration _configuration;

        // Verilen bilgilere göre senaryoyu test edebilmek amaçlı bir veri seti oluşturuldu.
        private JArray dummyInvoices = JArray.Parse(@"[
          {
            ""InvoiceHeader"": {
              ""InvoiceId"": ""SVS202300000001"",
              ""SenderTitle"": ""Gönderici Firma"",
              ""ReceiverTitle"": ""Alıcı Firma"",
              ""Date"": ""2023-01-05""
            },
            ""InvoiceLine"": [
              {
                ""Id"": 1,
                ""Name"": ""1.Ürün"",
                ""Quantity"": 5,
                ""UnitCode"": ""Adet"",
                ""UnitPrice"": 10
              },
              {
                ""Id"": 2,
                ""Name"": ""2.Ürün"",
                ""Quantity"": 2,
                ""UnitCode"": ""Litre"",
                ""UnitPrice"": 3
              },
              {
                ""Id"": 3,
                ""Name"": ""3.Ürün"",
                ""Quantity"": 25,
                ""UnitCode"": ""Kilogram"",
                ""UnitPrice"": 2
              }
            ]
          },
          {
            ""InvoiceHeader"": {
              ""InvoiceId"": ""SVS202300000002"",
              ""SenderTitle"": ""Gönderici Firma 2"",
              ""ReceiverTitle"": ""Alıcı Firma 2"",
              ""Date"": ""2023-02-05""
            },
            ""InvoiceLine"": [
              {
                ""Id"": 4,
                ""Name"": ""2.Ürün"",
                ""Quantity"": 10,
                ""UnitCode"": ""Adet"",
                ""UnitPrice"": 20
              },
              {
                ""Id"": 5,
                ""Name"": ""5.Ürün"",
                ""Quantity"": 4,
                ""UnitCode"": ""Litre"",
                ""UnitPrice"": 6
              },
              {
                ""Id"": 6,
                ""Name"": ""6.Ürün"",
                ""Quantity"": 50,
                ""UnitCode"": ""Kilogram"",
                ""UnitPrice"": 4
              }
            ]
          }
        ]");

        public InvoiceWorker(IUnitOfWork unitOfWork, MailService mailService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mailService = mailService;
            _configuration = configuration;
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public override async Task ExecuteAsync(int args)
        {
            // JSON belgesini işlemek için gelen json nesnesini önceden oluşturmuş olduğumuz InvoiceData nesnesine deserialize ediyoruz
            List<InvoiceData> dummyInvoicesData = dummyInvoices.ToObject<List<InvoiceData>>();

            for (int i = 0; i < dummyInvoicesData.Count; i++)
            {
                // Oluştuduğumuz dummy verileri ile fatura kayıt işlemini tamamlıyoruz
                var invoiceResult = await _unitOfWork.Invoice.AddInvoice(dummyInvoicesData[i]);

                // Yeni bir kayıt işlemi gerçekleşmediyse sıradaki faturaya geçilir
                if (invoiceResult.Data == null)
                {
                    continue;
                }

                if (invoiceResult.Success && invoiceResult.Data != null)
                {

                    // InvoiceLine verilerini kaydetme işlemini yapıyoruz
                    foreach (var invoiceLineData in dummyInvoicesData[i].InvoiceLine)
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
                }

                // Başarılı bir şekilde gerçekleşen kayıt işleminin ardından Mail içeriğini ayarlıyoruz
                var mail = new MailRequestDto
                {
                    ToMail = _configuration.GetSection("MailAddressToSend").Value,
                    Subject = "Yeni Fatura",
                    Body = string.Format("Faturanız başarılı bir şekilde oluşturuldu. Oluşturulan fatura no: {0}", invoiceResult.Data.ExternalInvoiceId)
                };

                // Mail gönderecek servisi tetikliyoruz
                await _mailService.SendEmailAsync(mail);

                // Serilog
                Log.Information("E-postalar gönderildi: {Timestamp}", DateTime.Now);
            }
        }
    }
}
