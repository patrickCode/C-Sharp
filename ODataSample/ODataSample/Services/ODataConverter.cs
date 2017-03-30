using System;
using Microsoft.Data.Edm;
using System.Web.Http.OData;
using ODataSample.Interfaces;
using Microsoft.Data.OData.Query;
using System.Web.Http.OData.Query;
using Microsoft.Data.OData.Query.SemanticAst;

namespace ODataSample.Services
{
    public class ODataConverter : IODataExpressionConverter
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
            var settings = new ODataValidationSettings()
            {
                AllowedLogicalOperators = AllowedLogicalOperators.Equal | AllowedLogicalOperators.And | AllowedLogicalOperators.Or,
                AllowedFunctions = AllowedFunctions.Any | AllowedFunctions.All
            };
            convertedFilterOption.Validate(settings);
            return convertedFilterOption;
        }

        public string Convert(string originalExpression)
        {
            var filterOption = new FilterQueryOption(originalExpression,
                new ODataQueryContext(_sourceModel, _sourceType));

            var converterFilterOption = Convert(filterOption);
            return converterFilterOption.RawValue;
        }

        private string ResolveLamdaNode(LambdaNode node)
        {
            BinaryOperatorNode expression = node.Body as BinaryOperatorNode;
            string alias = "";
            string propertyName = "";
            string parentPropertyName = "";
            string rootPropertyName = "";

            //Property at root
            if (expression.Left is NonentityRangeVariableReferenceNode)
            {
                var leftNode = expression.Left as NonentityRangeVariableReferenceNode;
                alias = leftNode.Name;

                if (node.Source is CollectionPropertyAccessNode)
                {
                    propertyName = (node.Source as CollectionPropertyAccessNode).Property.Name;
                }
            }
            else if (expression.Left is SingleValuePropertyAccessNode)
            {
                var leftNode = expression.Left as SingleValuePropertyAccessNode;
                if (leftNode.Source is NonentityRangeVariableReferenceNode)
                {
                    alias = (leftNode.Source as NonentityRangeVariableReferenceNode).Name;
                }
                else if (leftNode.Source is EntityRangeVariableReferenceNode)
                {
                    alias = (leftNode.Source as EntityRangeVariableReferenceNode).Name;
                }
                propertyName = leftNode.Property.Name;

                if (node.Source is CollectionPropertyAccessNode)
                {
                    parentPropertyName = (node.Source as CollectionPropertyAccessNode).Property.Name;
                    var sourceNode = node.Source as CollectionPropertyAccessNode;
                    if (sourceNode.Source is SingleValuePropertyAccessNode)
                    {
                        var rootNode = sourceNode.Source as SingleValuePropertyAccessNode;
                        rootPropertyName = rootNode.Property.Name;
                    }
                }
                else if (node.Source is CollectionNavigationNode)
                {
                    var sourceNode = node.Source as CollectionNavigationNode;
                    parentPropertyName = ((dynamic)(sourceNode.ItemType.Definition)).Name;
                    if (sourceNode.Source is SingleNavigationNode)
                    {
                        var rootNode = sourceNode.Source as SingleNavigationNode;
                        rootPropertyName = ((dynamic)(rootNode.EntityTypeReference.Definition)).Name;
                    }
                }

            }

            var rightExpression = (expression.Right as ConstantNode).LiteralText;

            var mappedPropertyName = _fieldMapper.Map(propertyName, parentPropertyName);
            var lamdaProperty = node.Kind == QueryNodeKind.Any ? "any" : "all";
            var expressionFormat = $"{mappedPropertyName}/{lamdaProperty}({alias}: {alias} eq {rightExpression})";
            return expressionFormat;
        }
        
        private string Resolve(SingleValueNode node)
        {
            BinaryOperatorNode expression;

            if (node is LambdaNode)
            {
                return ResolveLamdaNode(node as LambdaNode);
            }
            else if (node is ConvertNode)
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
                return CreateExpressionForBinaryNode(expression);
            }
            var leftExpression = Resolve(expression.Left);
            var rightExpression = Resolve(expression.Right);

            if (!(IsLeafNode(expression.Left) || IsAnyAllNode(expression.Left)))
                leftExpression = $"({leftExpression})";

            if (!(IsLeafNode(expression.Right) || IsAnyAllNode(expression.Right)))
                rightExpression = $"({rightExpression})";

            return $"{leftExpression} {GetODataOperator(expression.OperatorKind)} {rightExpression}";
        }

        private string CreateExpressionForBinaryNode(BinaryOperatorNode expression)
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

        private bool IsAnyAllNode(SingleValueNode node)
        {
            return ((node is AnyNode) || (node is AllNode));
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