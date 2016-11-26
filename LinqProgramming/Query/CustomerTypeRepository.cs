using System.Collections.Generic;

namespace Query
{
    public class CustomerTypeRepository
    {
        public List<CustomerType> Retreive()
        {
            return new List<CustomerType>()
            {
                new CustomerType()
                {
                    CustomerTypeId = 1,
                    CustomerTypeName = "DC Corporation",
                    DisplayOrder = 2
                },
                 new CustomerType()
                 {
                     CustomerTypeId = 2,
                     CustomerTypeName = "Marvel Industries",
                     DisplayOrder = 3
                 },
                    new CustomerType()
                 {
                     CustomerTypeId = 3,
                     CustomerTypeName = "Fox Enterprise",
                     DisplayOrder = 1
                 }
            };
        }
    }
}