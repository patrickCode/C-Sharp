using System.Collections.Generic;

namespace Query
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? CustomerTypeId { get; set; }
        public string EmailAdress { get; set; }
        public List<Invoice> Invoices
        {
            get
            {
                return new InvoiceRepository().Retreive(CustomerId);
            }
        }
    }
}