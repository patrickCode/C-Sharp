using ODataSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ODataSample.Services
{
    public class InMemoryProductsQueryService
    {
        public IQueryable<Product> Get()
        {
            return new List<Product>()
            {
                new Product()
                {
                    Id = 1,
                    Name = "P1",
                    Description = "Description 1"
                },
                new Product()
                {
                    Id = 2,
                    Name = "P2",
                    Description = "Description 2"
                }
            }
                .AsQueryable();
        }
    }
}