using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Query.Tests
{
    [TestClass]
    public class CustomerRepositoryTests
    {
        private CustomerRepository _repository;
        private List<Customer> _list;

        public CustomerRepositoryTests()
        {
            _repository = new CustomerRepository();
            _list = _repository.Retrieve();
        }
        [TestMethod]
        public void FindCustomerById()
        {
            //Arrange
            _repository = new CustomerRepository();

            //Act
            var result =_repository.FindById(_list, 1);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CustomerId);
            Assert.AreEqual("Bruce", result.FirstName);
            Assert.AreEqual("Wayne", result.LastName);
            Assert.AreEqual("batman@gotham.com", result.EmailAdress);
            Assert.AreEqual(1, result.CustomerTypeId);
        }

        [TestMethod]
        public void FindCustomer()
        {
            //Arrange
            _repository = new CustomerRepository();

            //Act
            var result =_repository.Find(_list, 1);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CustomerId);
            Assert.AreEqual("Bruce", result.FirstName);
            Assert.AreEqual("Wayne", result.LastName);
            Assert.AreEqual("batman@gotham.com", result.EmailAdress);
            Assert.AreEqual(1, result.CustomerTypeId);
        }

        [TestMethod]
        public void FindCustomerNotFound()
        {
            //Arrange
            _repository = new CustomerRepository();

            //Act
            var result =_repository.Find(_list, 101);

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void SortCustomerByName()
        {
            //Arrange
            _repository = new CustomerRepository();

            //Act
            var result =_repository.SortByName(_list);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Johnson", result.First().LastName);
        }

        [TestMethod]
        public void SortCustomerByNameReverse()
        {
            //Arrange
            _repository = new CustomerRepository();

            //Act
            var result = _repository.SortByNameReverse(_list);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Wayne", result.First().LastName);
        }

        [TestMethod]
        public void SortCustomerByType()
        {
            //Arrange
            _repository = new CustomerRepository();

            //Act
            var result = _repository.SortByCustomerType(_list);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.First().CustomerTypeId);   
        }

        [TestMethod]
        public void SortCustomerByTypeWithNullAtEnd()
        {
            //Arrange
            _repository = new CustomerRepository();

            //Act
            var result = _repository.SortByCustomerType(_list, false);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Last().CustomerTypeId);
        }

        [TestMethod]
        public void GetNames()
        {
            //Act
            var names = _repository.GetNames(_list);

            //Analyze
            foreach (var name in names)
            {
                Debug.WriteLine(name);
            }

            //Assert
            Assert.IsNotNull(names);
        }

        [TestMethod]
        public void GetNameAndEmail()
        {
            //Act
            var list = _repository.GetNamesAndEmail(_list);

            //Analyze
            foreach (var item in list)
            {
                Debug.WriteLine(item);
            }

            //Assert
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void GetNameAndType()
        {
            //Arrange
            var customerTypeRepository = new CustomerTypeRepository();
            var typeList = customerTypeRepository.Retreive();

            //Act
            var list = _repository.GetNameAndType(_list, typeList);

            //Analyze
            foreach (var item in list)
            {
                Debug.WriteLine(item);
            }

            //Assert
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void GetDueCustomer()
        {
            //Act
            var result = _repository.GetDueCustomers(_list);

            //Analyze
            foreach(var customer in result)
            {
                Debug.WriteLine(customer);
            }

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetDueByCustomertype()
        {
            //Arrange
            var customerTypeRepository = new CustomerTypeRepository();
            var types = customerTypeRepository.Retreive();

            //Act
            var result = _repository.GetInvoiceTotalByCustomerType(_list, types);
            foreach (var group in result)
            {
                Debug.WriteLine(group);
            }

            //Assert
            Assert.IsNotNull(result);
        }
    }
}