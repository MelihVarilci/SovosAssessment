using SovosAssessment.Domain.Entities.Common.Concrete;
using System.ComponentModel.DataAnnotations.Schema;

namespace SovosAssessment.Domain.Entities
{
    [Table("InvoiceLines")]

    public class InvoiceLine : BaseEntity
    {
        public virtual string Name { get; set; }
        
        public virtual int Quantity { get; set; }
        
        public virtual string UnitCode { get; set; }

        public virtual int UnitPrice { get; set; }

        public virtual int InvoiceId { get; set; }

        [ForeignKey("InvoiceId")]
        public Invoice InvoiceFk { get; set; }
    }
}
