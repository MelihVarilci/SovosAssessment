using SovosAssessment.Domain.Entities.Common.Concrete;
using System.ComponentModel.DataAnnotations.Schema;

namespace SovosAssessment.Domain.Entities
{
    [Table("Invoices")]
    public class Invoice : BaseEntity
    {
        public Invoice()
        {
            InvoiceLines = new HashSet<InvoiceLine>();
        }
        public virtual string ExternalInvoiceId { get; set; }
        public virtual string SenderTitle { get; set; }
        
        public virtual string ReceiverTitle { get; set; }
        
        public virtual DateTime Date { get; set; }

        public virtual ICollection<InvoiceLine> InvoiceLines { get; set; }
    }
}
