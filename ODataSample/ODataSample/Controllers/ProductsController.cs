using ODataSample.Interfaces;
using ODataSample.Models;
using ODataSample.Models.EDM;
using ODataSample.Services;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;

namespace ODataSample.Controllers
{
    [RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
        private readonly InMemoryProductsQueryService _queryService;
        private readonly IODataExpressionConverter _oDataConverter;
        public ProductsController(InMemoryProductsQueryService queryService, IODataExpressionConverter oDataConverter)
        {
            _queryService = queryService;
            _oDataConverter = oDataConverter;
        }

        public ProductsController()
        {
            _queryService = new InMemoryProductsQueryService();

            var fieldMapper = new ProductFieldMapper();
            var productEdmModel = Product.GetEdmModel();
            var productDocsEdmModel = ProductDoc.GetProductsModel();
            _oDataConverter = new ODataConverter(productEdmModel, typeof(Product), productDocsEdmModel, typeof(ProductDoc), fieldMapper);
        }

        [HttpGet]
        public IQueryable<Product> Get(ODataQueryOptions options, string idType = null)
        {
            //Validate the OData setting on the actual object
            var settings = new ODataValidationSettings()
            {
                AllowedLogicalOperators = AllowedLogicalOperators.Equal | AllowedLogicalOperators.And | AllowedLogicalOperators.Or,
                AllowedFunctions = AllowedFunctions.Any | AllowedFunctions.All
            };
            options.Validate(settings);
            //Convert the filter to flattened model
            var converterFilterOption = _oDataConverter.Convert(options.Filter);

            return _queryService.Get();
        }
    }
}