using MailKit.Net.Smtp;
using MailKit.Security;
using SovosAssessment.Application.DTOs;
using Microsoft.Extensions.Options;
using SovosAssessment.Infrastructure.Persistence.Repositories;
using MimeKit;

namespace SovosAssessment.Application.Abstractions.Services
{
    public class MailService
    {
        private readonly MailSettingsDto _mailSettings;

        public MailService(IOptions<MailSettingsDto> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MailRequestDto mailRequest)
        {
            // Maili yapılandırması
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailSettings.Mail)
            };

            email.To.Add(MailboxAddress.Parse(mailRequest.ToMail));
            email.Subject = mailRequest.Subject;

            var builder = new BodyBuilder();

            // Mailde ek'ler kısmı dolu ise
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }

            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();

            // SMTP ayarlarını appsettings.json dosyasındaki bilgiler ile ayarlıyoruz.
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

            // Mail gönderiliyor
            await smtp.SendAsync(email);
            
            smtp.Disconnect(true);
        }
    }
}
