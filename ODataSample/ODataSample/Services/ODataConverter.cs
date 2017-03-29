using System;
using Microsoft.Data.Edm;
using System.Web.Http.OData;
using ODataSample.Interfaces;
using Microsoft.Data.OData.Query;
using System.Web.Http.OData.Query;
using Microsoft.Data.OData.Query.SemanticAst;

namespace ODataSample.Services
{
    public class ODataConverter: IODataExpressionConverter
    {
        private IEdmModel _sourceModel;
        private Type _sourceType;

        private IEdmModel _targetModel;
        private Type _targetType;

        private IFieldMapper _fieldMapper;

        public ODataConverter(IEdmModel sourceModel, Type sourceType, IEdmModel targetModel, Type targetType, IFieldMapper fieldMapper)
        {
            _sourceModel = sourceModel;
            _sourceType = sourceType;

            _targetModel = targetModel;
            _targetType = targetType;

            _fieldMapper = fieldMapper;
        }

        public FilterQueryOption Convert(FilterQueryOption originalExpression)
        {
            var rawConverterExpression = Resolve(originalExpression.FilterClause.Expression);
            var convertedFilterOption = new FilterQueryOption(rawConverterExpression,
                new ODataQueryContext(_targetModel, _targetType));
            return convertedFilterOption;
        }

        public string Convert(string originalExpression)
        {
            var filterOption = new FilterQueryOption(originalExpression, 
                new ODataQueryContext(_sourceModel, _sourceType));

            var converterFilterOption = Convert(filterOption);
            return converterFilterOption.RawValue;
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

                if (leftNode.Source is SingleValuePropertyAccessNode)
                {
                    sourceName = (leftNode.Source as SingleValuePropertyAccessNode).Property.Name;
                }

                var docName = _fieldMapper.Map(leftNode.Property.Name, sourceName);
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