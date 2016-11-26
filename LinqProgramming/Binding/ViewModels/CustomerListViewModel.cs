using Binding.Model;
using Query;
using System.Collections.ObjectModel;
using System.Linq;

namespace Binding.ViewModels
{
    public class CustomerListViewModel: ViewModelBase
    {
        private ObservableCollection<CustomerModel> _customers;
        public ObservableCollection<CustomerModel> Customers
        {
            get
            {
                return _customers;
            }
            set
            {
                if (_customers != value)
                {
                    _customers = value;
                    NotifyPropertyChanged();
                }
            }
        }

        CustomerRepository customerRepository = new CustomerRepository();

        public CustomerListViewModel()
        {
            LoadData();
        }

        public void LoadData()
        {
            _customers = new ObservableCollection<CustomerModel>();
            var customerList = customerRepository.Retrieve();
            var customerTypeList = new CustomerTypeRepository().Retreive();
            //var customers = customerRepository.GetNameAndType(customerList, customerTypeList);
            var customers = customerList.Join(customerTypeList, //Iinner List
                customer => customer.CustomerTypeId,    //Property of the outer object on which Join will happen
                type => type.CustomerTypeId,    //Property of the inner object on which Join will happen
                (c, ct) => new CustomerModel()     //New Property that will be created
                {
                    CustomerName = string.Format("{0}, {1}", c.LastName, c.FirstName),
                    CustomerTypeName = ct.CustomerTypeName
                });

            foreach (var customer in customers)
            {
                _customers.Add(customer);
            }
        }
    }
}