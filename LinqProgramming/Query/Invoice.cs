using System;

namespace Query
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int CustomerId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool? IsPaid { get; set; }

        public decimal Amount { get; set; }
        public int NumberOfUnit { get; set; }
        public decimal DiscountPercet { get; set; }
        public decimal TotalAmount { get
            {
                return Amount - (Amount * DiscountPercet / 100);
            }
        }
    }
}