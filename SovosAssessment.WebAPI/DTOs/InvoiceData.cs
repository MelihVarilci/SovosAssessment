namespace SovosAssessment.WebAPI.DTOs
{
    public class InvoiceData
    {
        public InvoiceHeader InvoiceHeader { get; set; }
        public List<InvoiceLineData> InvoiceLine { get; set; }
    }
}
