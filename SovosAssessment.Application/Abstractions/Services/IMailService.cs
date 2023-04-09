using SovosAssessment.Application.DTOs;

namespace SovosAssessment.Infrastructure.Persistence.Repositories
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequestDto mailRequest);
    }
}
