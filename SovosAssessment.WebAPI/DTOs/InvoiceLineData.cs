﻿namespace SovosAssessment.WebAPI.DTOs
{
    public class InvoiceLineData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string UnitCode { get; set; }
        public int UnitPrice { get; set; }
    }
}
