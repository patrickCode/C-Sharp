using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.OData.Query;
using System.Web.Http.OData.Query.Validators;
using Microsoft.Data.OData.Query.SemanticAst;

namespace ODataSample.Validators
{
    public class CustomFilterValidator: FilterQueryValidator
    {
        public override void Validate(FilterQueryOption filterQueryOption, ODataValidationSettings settings)
        {
            ValidateQueryNode(filterQueryOption.FilterClause.Expression, settings);
        }

        public override void ValidateQueryNode(QueryNode node, ODataValidationSettings settings)
        {

            SingleValueNode singleNode = node as SingleValueNode;

            CollectionNode collectionNode = node as CollectionNode;

            base.ValidateQueryNode(node, settings);
        }

        
    }
}