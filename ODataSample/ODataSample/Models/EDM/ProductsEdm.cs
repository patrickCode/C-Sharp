using Microsoft.Data.Edm;
using Microsoft.Data.Edm.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.OData.Builder;

namespace ODataSample.Models.EDM
{
    public class ProductDocEdm
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string DetailsDescription { get; set; }
        public string DescriptionLanguageCode { get; set; }
        public string DescriptionLanguageName { get; set; }

        public static IEdmModel GetProductsModel()
        {   
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<ProductDocEdm>("ProductsEdm");
            return builder.GetEdmModel();
        }
    }
}