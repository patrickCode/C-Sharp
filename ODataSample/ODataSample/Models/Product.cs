using Microsoft.Data.Edm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.OData.Builder;

namespace ODataSample.Models
{
    public class Product
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Details Details { get; set; }

        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Product>("Product");
            return builder.GetEdmModel();
        }
    }

    public class Details
    {
        public string Description { get; set; }
        public Language Lang { get; set; }
    }

    public class Language
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}