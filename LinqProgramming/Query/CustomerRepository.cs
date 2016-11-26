using System.Linq;
using System.Collections.Generic;
using System;

namespace Query
{
    public class CustomerRepository
    {
        #region Basic
        //Declarative Query Syntax
        public Customer FindById(List<Customer> customers, int customerId)
        {
            //customer must support IEnumerable
            //Select projects each element of the resulting sequence into a new form. In this case the form is not being changes instead it is returing it as is.
            //Query returns an Enumerator which is used to visit each element of the result.
            //Linq uses deferred execution so the below statement is only defining the execution, the query itself is not executed until it's result is required.
            //Calling any operator over the query that requires for the query to iterate over its result will execute the query
            var query = from c in customers
                        where c.CustomerId == customerId
                        select c;

            //Query is executed here. Since we need only the first result the query will stop executing as soon as the first element is found.
            return query.FirstOrDefault();
        }
        #endregion

        #region Order
        //Linq Extension Methods
        //Linq adds extensions methods to any object that extends the IEnumerable
        public Customer Find(List<Customer> customers, int customerId)
        {
            return customers.FirstOrDefault(customer => customer.CustomerId == customerId);
        }

        public IEnumerable<Customer> SortByName(List<Customer> customers)
        {
            return customers.OrderBy(customer => customer.LastName)
                .ThenBy(customer => customer.FirstName);
        }

        public IEnumerable<Customer> SortByNameReverse(List<Customer> customers)
        {
            //return customers.OrderByDescending(customer => customer.LastName)
            //    .ThenByDescending(customer => customer.FirstName);

            //Same as above code
            return SortByName(customers).Reverse();

        }

        public IEnumerable<Customer> SortByCustomerType(List<Customer> customers, bool nullAtTop = true)
        {
            //Null by default goes first
            //Has Value will be true (1) if CustomerTypeId is not null
            if (nullAtTop)
                return customers.OrderBy(customer => customer.CustomerTypeId);
            else
                return customers.OrderByDescending(customer => customer.CustomerTypeId.HasValue)
                    .ThenBy(customer => customer.CustomerTypeId);
        }
        #endregion

        #region Projections
        public IEnumerable<string> GetNames(List<Customer> customers)
        {
            var query = customers.Select(customer => string.Format("{0}, {1}", customer.LastName, customer.FirstName));
            return query;
        }

        public IEnumerable<object> GetNamesAndEmail(List<Customer> customerList)
        {
            var query = customerList.Select(customer => new
            {
                Name = string.Format("{0}, {1}", customer.LastName, customer.FirstName),
                Email = customer.EmailAdress
            });
            return query;
        }

        public IEnumerable<object> GetNameAndType(List<Customer> customerList, List<CustomerType> customerTypeList)
        {
            //If customer type if is null for any custome then teh join will ignore that
            var query = customerList.Join(customerTypeList, //Iinner List
                customer => customer.CustomerTypeId,    //Property of the outer object on which Join will happen
                type => type.CustomerTypeId,    //Property of the inner object on which Join will happen
                (c, ct) => new      //New Property that will be created
                {
                    Name = string.Format("{0}, {1}", c.LastName, c.FirstName),
                    CustomerType = ct.CustomerTypeName
                });
            return query;
        }
        #endregion

        #region Parent Child
        public IEnumerable<object> GetDueCustomers(List<Customer> customerList)
        {
            //This one will only return a List of Invoices List. The select operator will return a List of whatever it is projecting and we are projecting a List on Invoices.
            //var query = customerList
            //    .Select(customer => customer.Invoices
            //                                  .Where(invoice => (invoice.IsPaid ?? false) == false));

            //Select Many will flatten out the list, so if we are selecting a list then it will flaten out the element of the list and return it. So the below one will return a List of Invoices.
            //var query = customerList
            //    .SelectMany(customer => customer.Invoices
            //                                  .Where(invoice => (invoice.IsPaid ?? false) == false));

            var query = customerList
                .SelectMany(customer => customer.Invoices
                                              .Where(invoice => (invoice.IsPaid ?? false) == false),
                                              (customer, invoice) => new
                                              {
                                                  Name = string.Format("{0}, {1}", customer.LastName, customer.FirstName),
                                                  DueDate = invoice.DueDate
                                              });

            //If we want only the customer and not a transformed object
            //var query = customerList
            //    .SelectMany(customer => customer.Invoices
            //                                  .Where(invoice => (invoice.IsPaid ?? false) == false),
            //                                  (customer, invoice) => customer);

            return query;
        }
        #endregion

        #region Analysis
        public IEnumerable<KeyValuePair<string, decimal>> GetInvoiceTotalByCustomerType(List<Customer> customerList, List<CustomerType> customerTypeList)
        {
            var customerTypeQuery = customerList.Join(customerTypeList,
                customer => customer.CustomerTypeId,
                type => type.CustomerTypeId,
                (c, ct) => new
                {
                    customer = c,
                    TypeName = ct.CustomerTypeName
                });

            var query = customerTypeQuery.GroupBy(customerType => customerType.TypeName,
                ct => ct.customer.Invoices.Sum(inv => inv.TotalAmount),
                (ct, invAmnt) => new KeyValuePair<string, decimal>(ct, invAmnt.Sum()));
            return query;
        }
        #endregion
        public List<Customer> Retrieve()
        {
            return new List<Customer>()
            {
                new Customer()
                {
                    CustomerId = 1,
                    FirstName = "Bruce",
                    LastName = "Wayne",
                    EmailAdress = "batman@gotham.com",
                    CustomerTypeId = 1
                },
                new Customer()
                {
                    CustomerId = 2,
                    FirstName = "Po",
                    LastName = "Johnson",
                    EmailAdress = "popo@johnson.com",
                    CustomerTypeId = null
                },
                new Customer()
                {
                    CustomerId = 3,
                    FirstName = "Pater",
                    LastName = "Parker",
                    EmailAdress = "spiderman@marvel.com",
                    CustomerTypeId = 2
                },
                new Customer()
                {
                    CustomerId = 4,
                    FirstName = "Tony",
                    LastName = "Stark",
                    EmailAdress = "ironman@marvel.com",
                    CustomerTypeId = 2
                },
                new Customer()
                {
                    CustomerId = 5,
                    FirstName = "Clark",
                    LastName = "Kent",
                    EmailAdress = "superman@metropolis.com",
                    CustomerTypeId = 1
                },
                new Customer()
                {
                    CustomerId = 6,
                    FirstName = "Martha",
                    LastName = "Kent",
                    EmailAdress = "martha@metropolis.com",
                    CustomerTypeId = null
                }
            };
        }
    }
}