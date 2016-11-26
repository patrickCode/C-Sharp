using System;
using System.Linq;
using System.Collections.Generic;

namespace Query
{
    public class InvoiceRepository
    {
        public List<Invoice> Retreive(int customerId)
        {
            return Retreive().Where(invoice => invoice.CustomerId == customerId).ToList();
        }
        public List<Invoice> Retreive()
        {
            return new List<Invoice>()
            {
                new Invoice()
                {
                    InvoiceId = 1,
                    CustomerId = 1,
                    DueDate = new DateTime(2016, 4, 16),
                    InvoiceDate = new DateTime(2016, 3, 1),
                    IsPaid = false,
                    Amount = 100.00M,
                    DiscountPercet = 0M,
                    NumberOfUnit = 12
                },
                new Invoice()
                {
                    InvoiceId = 2,
                    CustomerId = 3,
                    DueDate = new DateTime(2016, 3, 16),
                    InvoiceDate = new DateTime(2016, 3, 5),
                    IsPaid = true,
                    Amount = 156.99M,
                    DiscountPercet = 10M,
                    NumberOfUnit = 18
                },
                new Invoice()
                {
                    InvoiceId = 3,
                    CustomerId = 3,
                    DueDate = new DateTime(2015, 8, 16),
                    InvoiceDate = new DateTime(2014, 11, 13),
                    IsPaid = null,
                    Amount = 100.00M,
                    DiscountPercet = 4M,
                    NumberOfUnit = 4
                },
                new Invoice()
                {
                    InvoiceId = 4,
                    CustomerId = 2,
                    DueDate = new DateTime(2016, 12, 15),
                    InvoiceDate = new DateTime(2015, 12, 12),
                    IsPaid = false,
                    Amount = 750.00M,
                    DiscountPercet = 30M,
                    NumberOfUnit = 100
                },
                new Invoice()
                {
                    InvoiceId = 5,
                    CustomerId = 4,
                    DueDate = new DateTime(2016, 3, 6),
                    InvoiceDate = new DateTime(2016, 2, 1),
                    IsPaid = false,
                    Amount = 650.00M,
                    DiscountPercet = 15M,
                    NumberOfUnit = 75
                },
                new Invoice()
                {
                    InvoiceId = 6,
                    CustomerId = 1,
                    DueDate = new DateTime(2018, 7, 24),
                    InvoiceDate = new DateTime(2010, 9, 30),
                    IsPaid = true,
                    Amount = 500.00M,
                    DiscountPercet = 0M,
                    NumberOfUnit = 7
                },
                new Invoice()
                {
                    InvoiceId = 7,
                    CustomerId = 6,
                    DueDate = new DateTime(2016, 10, 11),
                    InvoiceDate = new DateTime(2013, 2, 3),
                    IsPaid = true,
                    Amount = 750.00M,
                    DiscountPercet = 30M,
                    NumberOfUnit = 100
                },
                new Invoice()
                {
                    InvoiceId = 8,
                    CustomerId = 5,
                    DueDate = new DateTime(2017, 3, 18),
                    InvoiceDate = new DateTime(2015, 1, 20),
                    IsPaid = false,
                    Amount = 199.00M,
                    DiscountPercet = 11M,
                    NumberOfUnit = 60
                },
                new Invoice()
                {
                    InvoiceId = 9,
                    CustomerId = 3,
                    DueDate = new DateTime(2016, 4, 27),
                    InvoiceDate = new DateTime(2016, 2, 17),
                    IsPaid = false,
                    Amount = 399.99M,
                    DiscountPercet = 10M,
                    NumberOfUnit = 65
                }
            };
        }
        public decimal CalculateTotalAmountInvoiced(List<Invoice> invoiceList)
        {
            return invoiceList.Sum(invoice => invoice.TotalAmount);
        }

        public decimal CalculateTotalUnitsSold(List<Invoice> invoiceList)
        {
            return invoiceList.Sum(invoice => invoice.NumberOfUnit);
        }

        public IEnumerable<dynamic> GetTotalByIsPaid(List<Invoice> invoiceList)
        {
            var query = invoiceList.GroupBy(invoice => invoice.IsPaid ?? false,  //Key on which grouping is done
                invoice => invoice.TotalAmount,  //Data which will be grouped
                (groupKey, invTotal) => new      //Entity that will be created
                {
                    Key = groupKey,
                    Total = invTotal.Sum()
                });

            foreach (var group in query)
            {
                Console.WriteLine(group);
            }
            return query;
        }

        public IEnumerable<object> GetTotalByIsPaidAndYear(List<Invoice> invoiceList)
        {

            var query = invoiceList.GroupBy(invoice => new      //The group by key is an anonymous type
            {
                IsPaid = invoice.IsPaid,
                Year = invoice.DueDate.Year
            },
                invoice => invoice.TotalAmount,
                (groupKey, invTotal) => new
                {
                    Key = groupKey,
                    Total = invTotal.Sum()
                });

            foreach (var group in query)
            {
                Console.WriteLine(group);
            }
            return query;
        }

        public decimal CalculateMeanOfDiscount(List<Invoice> invoiceList)
        {
            return invoiceList.Average(invoice => invoice.DiscountPercet);
        }

        public decimal CalculateMedianOfDiscount(List<Invoice> invoiceList)
        {
            var orderedList = invoiceList.OrderBy(i => i.DiscountPercet);
            var count = orderedList.Count();
            var medianPosition = count / 2;

            decimal median;
            if (count % 2 == 0)
                median = (orderedList.ElementAt(medianPosition).DiscountPercet + orderedList.ElementAt(medianPosition - 1).DiscountPercet) / 2;
            else
                median = orderedList.ElementAt(medianPosition).DiscountPercet;

            return median;
        }

        public decimal CalculateModeOfDiscount(List<Invoice> invoiceList)
        {
            var mode = invoiceList.GroupBy(i => i.DiscountPercet)
                 .OrderByDescending(group => group.Key)
                 .Select(group => group.Key)
                 .FirstOrDefault();
            return mode;
        }
    }
}