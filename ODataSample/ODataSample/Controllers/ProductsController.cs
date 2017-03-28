using Microsoft.Data.Edm.Library;
using Microsoft.Data.OData.Query;
using Microsoft.Data.OData.Query.SemanticAst;
using ODataSample.Models;
using ODataSample.Models.EDM;
using ODataSample.Services;
using ODataSample.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;

namespace ODataSample.Controllers
{
    [RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
        private readonly InMemoryProductsQueryService _queryService;
        public ProductsController(InMemoryProductsQueryService queryService)
        {
            _queryService = queryService;
        }

        public ProductsController()
        {
            _queryService = new InMemoryProductsQueryService();
        }

        [HttpGet]
        public IQueryable<Product> Get(ODataQueryOptions options, string idType = null)
        {
            //Validate the OData setting on the actual object
            var settings = new ODataValidationSettings()
            {
                AllowedLogicalOperators = AllowedLogicalOperators.Equal | AllowedLogicalOperators.And | AllowedLogicalOperators.Or
            };
            options.Validate(settings);

            //Convert the filter to flattened model
            var clause = options.Filter.FilterClause.Expression;
            var customRawExpression = Resolve(options.Filter.FilterClause.Expression);
            var edmModel = ProductDocEdm.GetProductsModel();
            var filterOption = new FilterQueryOption(customRawExpression, new ODataQueryContext(edmModel, typeof(ProductDocEdm)));

            return _queryService.Get();
        }

        private string Resolve(SingleValueNode node)
        {
            BinaryOperatorNode expression;
            if (node is ConvertNode)
            {
                var convertNode = node as ConvertNode;
                expression = (convertNode.Source) as BinaryOperatorNode;
            }
            else
            {
                expression = node as BinaryOperatorNode;
            }
            if (IsLeafNode(expression))
            {
                var leftNode = expression.Left as SingleValuePropertyAccessNode;
                var rightNode = expression.Right as ConstantNode;

                string sourceName = string.Empty;
                if (leftNode.Source is SingleNavigationNode)
                {
                    sourceName = ((dynamic)(leftNode.Source as SingleNavigationNode).EntityTypeReference.Definition).Name;
                }

                var docName = GetDocumentProperty(leftNode.Property.Name, sourceName);
                var val = rightNode.LiteralText;
                var op = expression.OperatorKind;

                var rawFilterExpression = $"{docName} {GetODataOperator(op)} {val}";

                return rawFilterExpression;
            }
            var leftExpression = Resolve(expression.Left);
            var rightExpression = Resolve(expression.Right);

            if (!IsLeafNode(expression.Left))
                leftExpression = $"({leftExpression})";

            if (!IsLeafNode(expression.Right))
                rightExpression = $"({rightExpression})";

            return $"{leftExpression} {GetODataOperator(expression.OperatorKind)} {rightExpression}";
        }

        private bool IsLeafNode(SingleValueNode node)
        {
            BinaryOperatorNode expression;
            if (node.Kind == QueryNodeKind.Convert)
            {
                var convertNode = node as ConvertNode;
                expression = (convertNode.Source as BinaryOperatorNode);
            }
            else if (node.Kind == QueryNodeKind.BinaryOperator)
            {
                expression = node as BinaryOperatorNode;
            }
            else
                return false;

            var leftNode = expression.Left;
            var rightNode = expression.Right;

            return leftNode is SingleValuePropertyAccessNode && rightNode is ConstantNode;
        }

        private string GetDocumentProperty(string propertName, string parentPropertName)
        {
            //TODO - Remove property naming and use OData references
            if (propertName == "Id") //Bad Practice - Not considering the name
                return "ProductId";
            if (propertName == "Name" && string.IsNullOrEmpty(parentPropertName))
                return "Name";
            if (propertName == "Description" && parentPropertName == "Details")
                return "DetailsDescription";
            if (propertName == "Description" && string.IsNullOrEmpty(parentPropertName))
                return "ShortDescription";
            if (propertName == "Name" && parentPropertName == "Language")
                return "DescriptionLanguageName";

            return "";
        }

        private string GetODataOperator(BinaryOperatorKind op)
        {
            if (op == BinaryOperatorKind.Equal)
                return "eq";
            if (op == BinaryOperatorKind.And)
                return "and";
            if (op == BinaryOperatorKind.Or)
                return "or";
            return "";
        }
    }
}
