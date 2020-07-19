using Microsoft.Data.Edm;
using System.Collections.Generic;
using System.Web.Http.OData.Builder;

namespace ODataSample.Models.EDM
{
    public class ProductDoc
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string DetailsDescription { get; set; }
        public string DescriptionLanguageCode { get; set; }
        public string DescriptionLanguageName { get; set; }
        public List<string> ProductTags { get; set; }
        public List<string> DetailReferenceNames { get; set; }
        public List<string> OrderLines { get; set; }

        public static IEdmModel GetProductsModel()
        {   
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<ProductDoc>("ProductsEdm");
            return builder.GetEdmModel();
        }
    }
}