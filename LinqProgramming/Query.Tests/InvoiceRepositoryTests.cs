using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Query.Tests
{
    [TestClass]
    public class InvoiceRepositoryTests
    {
        [TestMethod]
        public void CalculateTotalInvoicedAmount()
        {
            //Arrange
            var invoiceRepository = new InvoiceRepository();
            var invoiceList = invoiceRepository.Retreive();

            //Act
            var actual = invoiceRepository.CalculateTotalAmountInvoiced(invoiceList);

            //Analyze
            Debug.WriteLine(actual);

            //Assert
            Assert.IsTrue(actual > 0M);
        }

        [TestMethod]
        public void CalculateTotalUnits()
        {
            //Arrange
            var invoiceRepository = new InvoiceRepository();
            var invoiceList = invoiceRepository.Retreive();

            //Act
            var actual = invoiceRepository.CalculateTotalUnitsSold(invoiceList);

            //Analyze
            Debug.WriteLine(actual);

            //Assert
            Assert.IsTrue(actual > 0M);
        }

        [TestMethod]
        public void GetGroupedInvoiceAmountByIsPaid()
        {
            //Arrange
            var invoiceRepository = new InvoiceRepository();
            var invoiceList = invoiceRepository.Retreive();

            //Act
            var groups = invoiceRepository.GetTotalByIsPaid(invoiceList);

            //Assert
            Assert.IsNotNull(groups);
        }

        [TestMethod]
        public void GetGroupedInvoiceAmountByIsPaidAndYear()
        {
            //Arrange
            var invoiceRepository = new InvoiceRepository();
            var invoiceList = invoiceRepository.Retreive();

            //Act
            var groups = invoiceRepository.GetTotalByIsPaidAndYear(invoiceList);

            //Assert
            Assert.IsNotNull(groups);
        }

        [TestMethod]
        public void CalculateMeanOfDiscount()
        {
            //Arrange
            var invoiceRepository = new InvoiceRepository();
            var invoiceList = invoiceRepository.Retreive();

            //Act
            var mean = invoiceRepository.CalculateMeanOfDiscount(invoiceList);

            //Analyze
            Debug.WriteLine(mean);

            //Assert
            Assert.IsTrue(mean > 0M);
        }

        [TestMethod]
        public void CalculateMedianOfDiscount()
        {
            //Arrange
            var invoiceRepository = new InvoiceRepository();
            var invoiceList = invoiceRepository.Retreive();

            //Act
            var median = invoiceRepository.CalculateMedianOfDiscount(invoiceList);

            //Analyze
            Debug.WriteLine(median);

            //Assert
            Assert.IsTrue(median > 0M);
        }

        [TestMethod]
        public void CalculateModeOfDiscount()
        {
            //Arrange
            var invoiceRepository = new InvoiceRepository();
            var invoiceList = invoiceRepository.Retreive();

            //Act
            var mode = invoiceRepository.CalculateModeOfDiscount(invoiceList);

            //Analyze
            Debug.WriteLine(mode);

            //Assert
            Assert.AreEqual(mode, 30);
        }
    }
}
