using Microsoft.AspNetCore.Http;

namespace SovosAssessment.Application.DTOs
{
    public class MailRequestDto
    {
        public string ToMail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}
