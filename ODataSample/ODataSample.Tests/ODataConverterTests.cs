using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ODataSample.Interfaces;
using ODataSample.Services;
using ODataSample.Models;
using ODataSample.Models.EDM;
using Microsoft.Data.OData;
using ODataSample.Interfaces.Fakes;

namespace ODataSample.Tests
{
    [TestClass]
    public class ODataConverterTests
    {
        private IFieldMapper _fieldMapper;
        private IODataExpressionConverter _odataConverter;
        public ODataConverterTests()
        {
            _fieldMapper = new ProductFieldMapper();
            var productEdmModel = Product.GetEdmModel();
            var productDocsEdmModel = ProductDoc.GetProductsModel();
            _odataConverter = new ODataConverter(productEdmModel, typeof(Product), productDocsEdmModel, typeof(ProductDoc), _fieldMapper);
        }

        #region Single Expression
        [TestMethod]
        public void ConvertSingleExpression()
        {
            const string originalExpression = "Name eq 'TestName'";
            const string expectedExpression = "Name eq 'TestName'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertSingleExpression_WithPropertyNameChanged()
        {
            const string originalExpression = "Id eq 786";
            const string expectedExpression = "ProductId eq 786";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertSingleExpression_With1LevelNesting()
        {
            const string originalExpression = "Details/Description eq 'Dummy_Desc'";
            const string expectedExpression = "DetailsDescription eq 'Dummy_Desc'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertSingleExpression_With2LevelNesting()
        {
            const string originalExpression = "Details/Lang/Name eq 'English'";
            const string expectedExpression = "DescriptionLanguageName eq 'English'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        #endregion

        #region Multiple Expression

        #region Double Expression
        [TestMethod]
        public void ConvertSimpleDoubleExpression_WithAnd()
        {
            const string originalExpression = "Name eq 'DummyName' and Details/Lang/Name eq 'English'";
            const string expectedExpression = "Name eq 'DummyName' and DescriptionLanguageName eq 'English'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertSimpleDoubleExpression_WithOr()
        {
            const string originalExpression = "Id eq 907 or Details/Lang/Name eq 'English'";
            const string expectedExpression = "ProductId eq 907 or DescriptionLanguageName eq 'English'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertSimpleDoubleExpression_WithParanthesis_ShouldIgnoreParanthesis()
        {
            const string originalExpression = "(Name eq 'DummyName') and (Details/Lang/Name eq 'English')";
            const string expectedExpression = "Name eq 'DummyName' and DescriptionLanguageName eq 'English'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }
        #endregion

        #region Triple Expression
        [TestMethod]
        public void ConvertTripleExpression_WithAnd()
        {
            const string originalExpression = "Name eq 'DummyName' and Details/Lang/Name eq 'English' and Details/Description eq 'Desc'";
            const string expectedExpression = "(Name eq 'DummyName' and DescriptionLanguageName eq 'English') and DetailsDescription eq 'Desc'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertTripleExpression_WithOr()
        {
            const string originalExpression = "Name eq 'DummyName' or Details/Lang/Name eq 'English' or Details/Description eq 'Desc'";
            const string expectedExpression = "(Name eq 'DummyName' or DescriptionLanguageName eq 'English') or DetailsDescription eq 'Desc'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertTripleExpression_WithAnd_WithOr()
        {
            const string originalExpression = "Name eq 'DummyName' and Details/Lang/Name eq 'English' or Details/Description eq 'Desc'";
            const string expectedExpression = "(Name eq 'DummyName' and DescriptionLanguageName eq 'English') or DetailsDescription eq 'Desc'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertTripleExpression_WithOr_WithAnd()
        {
            const string originalExpression = "Name eq 'DummyName' or Details/Lang/Name eq 'English' and Details/Description eq 'Desc'";
            const string expectedExpression = "Name eq 'DummyName' or (DescriptionLanguageName eq 'English' and DetailsDescription eq 'Desc')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertTripleExpression_WithOr_Paranthesis_WithAnd()
        {
            const string originalExpression = "(Name eq 'DummyName' or Details/Lang/Name eq 'English') and Details/Description eq 'Desc'";
            const string expectedExpression = "(Name eq 'DummyName' or DescriptionLanguageName eq 'English') and DetailsDescription eq 'Desc'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }
        #endregion

        #region Quadra Expression
        [TestMethod]
        public void ConvertQuadraExpression_WithAnd()
        {
            const string originalExpression = "Name eq 'DummyName' and Details/Lang/Name eq 'English' and Details/Description eq 'Desc' and Description eq 'Desc_1'";
            const string expectedExpression = "((Name eq 'DummyName' and DescriptionLanguageName eq 'English') and DetailsDescription eq 'Desc') and ShortDescription eq 'Desc_1'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertQuadraExpression_WithAnd_WithParanthesisInMiddle()
        {
            const string originalExpression = "Name eq 'DummyName' and (Details/Lang/Name eq 'English' and Details/Description eq 'Desc') and Description eq 'Desc_1'";
            const string expectedExpression = "(Name eq 'DummyName' and (DescriptionLanguageName eq 'English' and DetailsDescription eq 'Desc')) and ShortDescription eq 'Desc_1'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertQuadraExpression_WithAnd_WithParanthesisAtLast()
        {
            const string originalExpression = "Name eq 'DummyName' and Details/Lang/Name eq 'English' and (Details/Description eq 'Desc' and Description eq 'Desc_1')";
            const string expectedExpression = "(Name eq 'DummyName' and DescriptionLanguageName eq 'English') and (DetailsDescription eq 'Desc' and ShortDescription eq 'Desc_1')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertQuadraExpression_WithOr()
        {
            const string originalExpression = "Name eq 'DummyName' or Details/Lang/Name eq 'English' or Details/Description eq 'Desc' or Description eq 'Desc_1'";
            const string expectedExpression = "((Name eq 'DummyName' or DescriptionLanguageName eq 'English') or DetailsDescription eq 'Desc') or ShortDescription eq 'Desc_1'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertQuadraExpression_WithAnd_WithOrInMiddle()
        {
            const string originalExpression = 
                "Name eq 'DummyName' and Details/Lang/Name eq 'English' or Details/Description eq 'Desc' and Description eq 'Desc_1'";
            const string expectedExpression = 
                "(Name eq 'DummyName' and DescriptionLanguageName eq 'English') or (DetailsDescription eq 'Desc' and ShortDescription eq 'Desc_1')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertQuadraExpression_WithAnd_WithOrAtFirst()
        {
            const string originalExpression =
                "Name eq 'DummyName' or Details/Lang/Name eq 'English' and Details/Description eq 'Desc' and Description eq 'Desc_1'";
            const string expectedExpression =
                "Name eq 'DummyName' or ((DescriptionLanguageName eq 'English' and DetailsDescription eq 'Desc') and ShortDescription eq 'Desc_1')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertQuadraExpression_WithAnd_WithOrAtFirst_OrHigherPreference()
        {
            const string originalExpression =
                "(Name eq 'DummyName' or Details/Lang/Name eq 'English') and Details/Description eq 'Desc' and Description eq 'Desc_1'";
            const string expectedExpression =
                "((Name eq 'DummyName' or DescriptionLanguageName eq 'English') and DetailsDescription eq 'Desc') and ShortDescription eq 'Desc_1'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertQuadraExpression_WithAnd_WithOrAtLast()
        {
            const string originalExpression =
                "Name eq 'DummyName' and Details/Lang/Name eq 'English' and Details/Description eq 'Desc' or Description eq 'Desc_1'";
            const string expectedExpression =
                "((Name eq 'DummyName' and DescriptionLanguageName eq 'English') and DetailsDescription eq 'Desc') or ShortDescription eq 'Desc_1'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertQuadraExpression_WithAnd_WithOrAtLast_WithOrGivenHigherPreference()
        {
            const string originalExpression =
                "Name eq 'DummyName' and Details/Lang/Name eq 'English' and (Details/Description eq 'Desc' or Description eq 'Desc_1')";
            const string expectedExpression =
                "(Name eq 'DummyName' and DescriptionLanguageName eq 'English') and (DetailsDescription eq 'Desc' or ShortDescription eq 'Desc_1')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertQuadraExpression_WithOr_WithAndInMiddle()
        {
            const string originalExpression =
                "Name eq 'DummyName' or Details/Lang/Name eq 'English' and Details/Description eq 'Desc' or Description eq 'Desc_1'";
            const string expectedExpression =
                "(Name eq 'DummyName' or (DescriptionLanguageName eq 'English' and DetailsDescription eq 'Desc')) or ShortDescription eq 'Desc_1'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertQuadraExpression_WithOr_WithAndInMiddle_OrHigherPreference()
        {
            const string originalExpression =
                "(Name eq 'DummyName' or Details/Lang/Name eq 'English') and (Details/Description eq 'Desc' or Description eq 'Desc_1')";
            const string expectedExpression =
                "(Name eq 'DummyName' or DescriptionLanguageName eq 'English') and (DetailsDescription eq 'Desc' or ShortDescription eq 'Desc_1')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertQuadraExpression_WithOr_WithAndAtFirst()
        {
            const string originalExpression =
                "Name eq 'DummyName' and Details/Lang/Name eq 'English' or Details/Description eq 'Desc' or Description eq 'Desc_1'";
            const string expectedExpression =
                "((Name eq 'DummyName' and DescriptionLanguageName eq 'English') or DetailsDescription eq 'Desc') or ShortDescription eq 'Desc_1'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertQuadraExpression_WithOr_WithAndAtLast()
        {
            const string originalExpression =
                "Name eq 'DummyName' or Details/Lang/Name eq 'English' or Details/Description eq 'Desc' and Description eq 'Desc_1'";
            const string expectedExpression =
                "(Name eq 'DummyName' or DescriptionLanguageName eq 'English') or (DetailsDescription eq 'Desc' and ShortDescription eq 'Desc_1')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }
        #endregion

        #region Penta Expression
        [TestMethod]
        public void ConvertPentaExpression_WithAnd()
        {
            const string originalExpression = "Name eq 'DummyName' and Details/Lang/Name eq 'English' and Details/Description eq 'Desc' and Description eq 'Desc_1' and Id eq 1652";
            const string expectedExpression = "(((Name eq 'DummyName' and DescriptionLanguageName eq 'English') and DetailsDescription eq 'Desc') and ShortDescription eq 'Desc_1') and ProductId eq 1652";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertPentaExpression_WithOr()
        {
            const string originalExpression = "Name eq 'DummyName' or Details/Lang/Name eq 'English' or Details/Description eq 'Desc' or Description eq 'Desc_1' or Id eq 1652";
            const string expectedExpression = "(((Name eq 'DummyName' or DescriptionLanguageName eq 'English') or DetailsDescription eq 'Desc') or ShortDescription eq 'Desc_1') or ProductId eq 1652";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertPentaExpression_WithAnd_WithOr_OrAtFirst()
        {
            const string originalExpression =
                "Name eq 'DummyName' or Details/Lang/Name eq 'English' and Details/Description eq 'Desc' and Description eq 'Desc_1' and Id eq 1652";
            const string expectedExpression =
                "Name eq 'DummyName' or (((DescriptionLanguageName eq 'English' and DetailsDescription eq 'Desc') and ShortDescription eq 'Desc_1') and ProductId eq 1652)";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertPentaExpression_WithAnd_WithOr_OrAtFirst_HigherPreferenceWithOr()
        {
            const string originalExpression =
                "(Name eq 'DummyName' or Details/Lang/Name eq 'English') and Details/Description eq 'Desc' and Description eq 'Desc_1' and Id eq 1652";
            const string expectedExpression =
                "(((Name eq 'DummyName' or DescriptionLanguageName eq 'English') and DetailsDescription eq 'Desc') and ShortDescription eq 'Desc_1') and ProductId eq 1652";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertPentaExpression_WithAnd_WithOr_OrAtLast()
        {
            const string originalExpression =
                "Name eq 'DummyName' and Details/Lang/Name eq 'English' and Details/Description eq 'Desc' and Description eq 'Desc_1' or Id eq 1652";
            const string expectedExpression =
                "(((Name eq 'DummyName' and DescriptionLanguageName eq 'English') and DetailsDescription eq 'Desc') and ShortDescription eq 'Desc_1') or ProductId eq 1652";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertPentaExpression_WithAnd_WithOr_OrAtLast_HigherPrefereneWithOr()
        {
            const string originalExpression =
                "Name eq 'DummyName' and Details/Lang/Name eq 'English' and Details/Description eq 'Desc' and (Description eq 'Desc_1' or Id eq 1652)";
            const string expectedExpression =
                "((Name eq 'DummyName' and DescriptionLanguageName eq 'English') and DetailsDescription eq 'Desc') and (ShortDescription eq 'Desc_1' or ProductId eq 1652)";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertPentaExpression_WithAnd_WithOr_OrInMiddle()
        {
            const string originalExpression = 
                "Name eq 'DummyName' and Details/Lang/Name eq 'English' or Details/Description eq 'Desc' and Description eq 'Desc_1' and Id eq 1652";
            const string expectedExpression = 
                "(Name eq 'DummyName' and DescriptionLanguageName eq 'English') or ((DetailsDescription eq 'Desc' and ShortDescription eq 'Desc_1') and ProductId eq 1652)";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertPentaExpression_WithAnd_WithOr_OrInMiddle_2()
        {
            const string originalExpression =
                "Name eq 'DummyName' and Details/Lang/Name eq 'English' and Details/Description eq 'Desc' or Description eq 'Desc_1' and Id eq 1652";
            const string expectedExpression =
                "((Name eq 'DummyName' and DescriptionLanguageName eq 'English') and DetailsDescription eq 'Desc') or (ShortDescription eq 'Desc_1' and ProductId eq 1652)";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertPentaExpression_WithAnd_WithOr_Alternate()
        {
            const string originalExpression = 
                "Name eq 'DummyName' and Details/Lang/Name eq 'English' or Details/Description eq 'Desc' and Description eq 'Desc_1' or Id eq 1652";
            const string expectedExpression = 
                "((Name eq 'DummyName' and DescriptionLanguageName eq 'English') or (DetailsDescription eq 'Desc' and ShortDescription eq 'Desc_1')) or ProductId eq 1652";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertPentaExpression_WithAnd_WithOr_Alternate_HigherPreferenceWithOr()
        {
            const string originalExpression =
                "Name eq 'DummyName' and (Details/Lang/Name eq 'English' or Details/Description eq 'Desc') and (Description eq 'Desc_1' or Id eq 1652)";
            const string expectedExpression =
                "(Name eq 'DummyName' and (DescriptionLanguageName eq 'English' or DetailsDescription eq 'Desc')) and (ShortDescription eq 'Desc_1' or ProductId eq 1652)";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertPentaExpression_WithOr_WithAnd_Alternate()
        {
            const string originalExpression =
                "Name eq 'DummyName' or Details/Lang/Name eq 'English' and Details/Description eq 'Desc' or Description eq 'Desc_1' and Id eq 1652";
            const string expectedExpression =
                "(Name eq 'DummyName' or (DescriptionLanguageName eq 'English' and DetailsDescription eq 'Desc')) or (ShortDescription eq 'Desc_1' and ProductId eq 1652)";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertPentaExpression_WithOr_WithAnd_Alternate_HigherPreferneceWithOr()
        {
            const string originalExpression =
                "(Name eq 'DummyName' or Details/Lang/Name eq 'English') and (Details/Description eq 'Desc' or Description eq 'Desc_1') and Id eq 1652";
            const string expectedExpression =
                "((Name eq 'DummyName' or DescriptionLanguageName eq 'English') and (DetailsDescription eq 'Desc' or ShortDescription eq 'Desc_1')) and ProductId eq 1652";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);

            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }
        #endregion
        #endregion

        #region Multi-Valued
        #region Any
        #region Single Expression
        [TestMethod]
        public void SingleAnyExpression()
        {
            const string originalExpression = "Tags/any(t: t eq 'Dummy')";
            const string expectedExpression = "ProductTags/any(t: t eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void SingleAnyExpression_WithSingleNestedProperties()
        {
            const string originalExpression = "OrderLines/any(o: o/Name eq 'Dummy')";
            const string expectedExpression = "OrderLines/any(o: o eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void SingleAnyExpression_WithDoubleNestedProperties()
        {
            const string originalExpression = "Details/References/any(r: r/Name eq 'Dummy')";
            const string expectedExpression = "DetailReferenceNames/any(r: r eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }
        #endregion

        #region Multiple Expression
        [TestMethod]
        public void DoubleAnyExpression_WithAnd()
        {
            const string originalExpression = "Tags/any(t: t eq 'Dummy') and OrderLines/any(o: o/Name eq 'Dummy')";
            const string expectedExpression = "ProductTags/any(t: t eq 'Dummy') and OrderLines/any(o: o eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void DoubleAnyExpression_WithAnd_ShouldParanthesis()
        {
            const string originalExpression = "(Tags/any(t: t eq 'Dummy') and OrderLines/any(o: o/Name eq 'Dummy'))";
            const string expectedExpression = "ProductTags/any(t: t eq 'Dummy') and OrderLines/any(o: o eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void DoubleAnyExpression_WithOr()
        {
            const string originalExpression = "Tags/any(t: t eq 'Dummy') or OrderLines/any(o: o/Name eq 'Dummy')";
            const string expectedExpression = "ProductTags/any(t: t eq 'Dummy') or OrderLines/any(o: o eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void TripleAnyExpression_WithAnd()
        {
            const string originalExpression = "Tags/any(t: t eq 'Dummy') and OrderLines/any(o: o/Name eq 'Dummy') and Details/References/any(r: r/Name eq 'Dummy')";
            const string expectedExpression = "(ProductTags/any(t: t eq 'Dummy') and OrderLines/any(o: o eq 'Dummy')) and DetailReferenceNames/any(r: r eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void TripleAnyExpression_WithOr()
        {
            const string originalExpression = "Tags/any(t: t eq 'Dummy') or OrderLines/any(o: o/Name eq 'Dummy') or Details/References/any(r: r/Name eq 'Dummy')";
            const string expectedExpression = "(ProductTags/any(t: t eq 'Dummy') or OrderLines/any(o: o eq 'Dummy')) or DetailReferenceNames/any(r: r eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void TripleAnyExpression_WithAnd_WithOr()
        {
            const string originalExpression = 
                "Tags/any(t: t eq 'Dummy') and OrderLines/any(o: o/Name eq 'Dummy') or Details/References/any(r: r/Name eq 'Dummy')";
            const string expectedExpression = 
                "(ProductTags/any(t: t eq 'Dummy') and OrderLines/any(o: o eq 'Dummy')) or DetailReferenceNames/any(r: r eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void TripleAnyExpression_WithAnd_WithOr_HigherPreferenceWithOr()
        {
            const string originalExpression =
                "Tags/any(t: t eq 'Dummy') and (OrderLines/any(o: o/Name eq 'Dummy') or Details/References/any(r: r/Name eq 'Dummy'))";
            const string expectedExpression =
                "ProductTags/any(t: t eq 'Dummy') and (OrderLines/any(o: o eq 'Dummy') or DetailReferenceNames/any(r: r eq 'Dummy'))";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void TripleAnyExpression_WithOr_WithAnd()
        {
            const string originalExpression =
                "Tags/any(t: t eq 'Dummy') or OrderLines/any(o: o/Name eq 'Dummy') and Details/References/any(r: r/Name eq 'Dummy')";
            const string expectedExpression =
                "ProductTags/any(t: t eq 'Dummy') or (OrderLines/any(o: o eq 'Dummy') and DetailReferenceNames/any(r: r eq 'Dummy'))";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void TripleAnyExpression_WithOr_WithAnd_HigherPreferenceWithOr()
        {
            const string originalExpression =
                "(Tags/any(t: t eq 'Dummy') or OrderLines/any(o: o/Name eq 'Dummy')) and Details/References/any(r: r/Name eq 'Dummy')";
            const string expectedExpression =
                "(ProductTags/any(t: t eq 'Dummy') or OrderLines/any(o: o eq 'Dummy')) and DetailReferenceNames/any(r: r eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }
        #endregion
        #endregion

        #region All
        #region Single Expression
        [TestMethod]
        public void SingleAllExpression()
        {
            const string originalExpression = "Tags/all(t: t eq 'Dummy')";
            const string expectedExpression = "ProductTags/all(t: t eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void SingleAllExpression_WithSingleNestedProperties()
        {
            const string originalExpression = "OrderLines/all(o: o/Name eq 'Dummy')";
            const string expectedExpression = "OrderLines/all(o: o eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void SingleAllExpression_WithDoubleNestedProperties()
        {
            const string originalExpression = "Details/References/all(r: r/Name eq 'Dummy')";
            const string expectedExpression = "DetailReferenceNames/all(r: r eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }
        #endregion

        #region Multiple Expression
        [TestMethod]
        public void DoubleAllExpression_WithAnd()
        {
            const string originalExpression = "Tags/all(t: t eq 'Dummy') and OrderLines/all(o: o/Name eq 'Dummy')";
            const string expectedExpression = "ProductTags/all(t: t eq 'Dummy') and OrderLines/all(o: o eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void DoubleAllExpression_WithAnd_ShouldParanthesis()
        {
            const string originalExpression = "(Tags/all(t: t eq 'Dummy') and OrderLines/all(o: o/Name eq 'Dummy'))";
            const string expectedExpression = "ProductTags/all(t: t eq 'Dummy') and OrderLines/all(o: o eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void DoubleAllExpression_WithOr()
        {
            const string originalExpression = "Tags/all(t: t eq 'Dummy') or OrderLines/all(o: o/Name eq 'Dummy')";
            const string expectedExpression = "ProductTags/all(t: t eq 'Dummy') or OrderLines/all(o: o eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void TripleAllExpression_WithAnd()
        {
            const string originalExpression = "Tags/all(t: t eq 'Dummy') and OrderLines/all(o: o/Name eq 'Dummy') and Details/References/all(r: r/Name eq 'Dummy')";
            const string expectedExpression = "(ProductTags/all(t: t eq 'Dummy') and OrderLines/all(o: o eq 'Dummy')) and DetailReferenceNames/all(r: r eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void TripleAllExpression_WithOr()
        {
            const string originalExpression = "Tags/all(t: t eq 'Dummy') or OrderLines/all(o: o/Name eq 'Dummy') or Details/References/all(r: r/Name eq 'Dummy')";
            const string expectedExpression = "(ProductTags/all(t: t eq 'Dummy') or OrderLines/all(o: o eq 'Dummy')) or DetailReferenceNames/all(r: r eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void TripleAllExpression_WithAnd_WithOr()
        {
            const string originalExpression =
                "Tags/all(t: t eq 'Dummy') and OrderLines/all(o: o/Name eq 'Dummy') or Details/References/all(r: r/Name eq 'Dummy')";
            const string expectedExpression =
                "(ProductTags/all(t: t eq 'Dummy') and OrderLines/all(o: o eq 'Dummy')) or DetailReferenceNames/all(r: r eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void TripleAllExpression_WithAnd_WithOr_HigherPreferenceWithOr()
        {
            const string originalExpression =
                "Tags/all(t: t eq 'Dummy') and (OrderLines/all(o: o/Name eq 'Dummy') or Details/References/all(r: r/Name eq 'Dummy'))";
            const string expectedExpression =
                "ProductTags/all(t: t eq 'Dummy') and (OrderLines/all(o: o eq 'Dummy') or DetailReferenceNames/all(r: r eq 'Dummy'))";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void TripleAllExpression_WithOr_WithAnd()
        {
            const string originalExpression =
                "Tags/all(t: t eq 'Dummy') or OrderLines/all(o: o/Name eq 'Dummy') and Details/References/all(r: r/Name eq 'Dummy')";
            const string expectedExpression =
                "ProductTags/all(t: t eq 'Dummy') or (OrderLines/all(o: o eq 'Dummy') and DetailReferenceNames/all(r: r eq 'Dummy'))";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void TripleAllExpression_WithOr_WithAnd_HigherPreferenceWithOr()
        {
            const string originalExpression =
                "(Tags/all(t: t eq 'Dummy') or OrderLines/all(o: o/Name eq 'Dummy')) and Details/References/all(r: r/Name eq 'Dummy')";
            const string expectedExpression =
                "(ProductTags/all(t: t eq 'Dummy') or OrderLines/all(o: o eq 'Dummy')) and DetailReferenceNames/all(r: r eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }
        #endregion
        #endregion
        #endregion

        #region Multiple Paranthesis
        [TestMethod]
        public void ConvertPentaExpressions_With2Paranthesis()
        {
            const string originalExpression =
                "(Id eq 456 or Tags/any(t: t eq 'Dummy')) and (Details/Description eq 'Desc' or OrderLines/all(o: o/Name eq 'order')) and Name eq 'Dummy'";
            const string expectedExpression =
                "((ProductId eq 456 or ProductTags/any(t: t eq 'Dummy')) and (DetailsDescription eq 'Desc' or OrderLines/all(o: o eq 'order'))) and Name eq 'Dummy'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertPentaExpressions_With3Paranthesis_Nested()
        {
            const string originalExpression =
                "(Id eq 456 or Tags/any(t: t eq 'Dummy')) and ((Details/Description eq 'Desc' or OrderLines/all(o: o/Name eq 'order')) or Name eq 'Dummy')";
            const string expectedExpression =
                "(ProductId eq 456 or ProductTags/any(t: t eq 'Dummy')) and ((DetailsDescription eq 'Desc' or OrderLines/all(o: o eq 'order')) or Name eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }
        #endregion

        #region Mixed Expressions
        [TestMethod]
        public void ConvertDoubleMixedExpression_WithAny_WithAnd()
        {
            const string originalExpression = "Tags/any(t: t eq 'Dummy') and Name eq 'Test_Name'";
            const string expectedExpression = "ProductTags/any(t: t eq 'Dummy') and Name eq 'Test_Name'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertDoubleMixedExpression_WithAny_WithAll()
        {
            const string originalExpression = "Tags/any(t: t eq 'Dummy') and OrderLines/all(o:o/Name eq 'order')";
            const string expectedExpression = "ProductTags/any(t: t eq 'Dummy') and OrderLines/all(o: o eq 'order')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertDoubleMixedExpression_WithAnd_WithAny()
        {
            const string originalExpression = "Id eq 456 and Tags/any(t: t eq 'Dummy')";
            const string expectedExpression = "ProductId eq 456 and ProductTags/any(t: t eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertTripleMixedExpression_WithAnd_WithAny()
        {
            const string originalExpression = "Id eq 456 and Details/Description eq 'Desc' and Tags/any(t: t eq 'Dummy')";
            const string expectedExpression = "(ProductId eq 456 and DetailsDescription eq 'Desc') and ProductTags/any(t: t eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertTripleMixedExpression_WithAnd_WithAny_WithAll()
        {
            const string originalExpression = "Id eq 456 and Tags/any(t: t eq 'Dummy') and OrderLines/all(o: o/Name eq 'order')";
            const string expectedExpression = "(ProductId eq 456 and ProductTags/any(t: t eq 'Dummy')) and OrderLines/all(o: o eq 'order')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertTripleMixedExpression_WithAnd_WithAny_ParanthesisAtLast()
        {
            const string originalExpression = "Id eq 456 and (Details/Description eq 'Desc' and Tags/any(t: t eq 'Dummy'))";
            const string expectedExpression = "ProductId eq 456 and (DetailsDescription eq 'Desc' and ProductTags/any(t: t eq 'Dummy'))";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertTripleMixedExpression_WithAnd_WithAny_AnyInMiddle()
        {
            const string originalExpression = "Id eq 456 and Tags/any(t: t eq 'Dummy') and Details/Description eq 'Desc'";
            const string expectedExpression = "(ProductId eq 456 and ProductTags/any(t: t eq 'Dummy')) and DetailsDescription eq 'Desc'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertPentaMixedExpression()
        {
            const string originalExpression = 
                "Id eq 456 and Tags/any(t: t eq 'Dummy') and Details/Description eq 'Desc' or OrderLines/all(o: o/Name eq 'order') and Name eq 'Dummy'";
            const string expectedExpression = 
                "((ProductId eq 456 and ProductTags/any(t: t eq 'Dummy')) and DetailsDescription eq 'Desc') or (OrderLines/all(o: o eq 'order') and Name eq 'Dummy')";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }

        [TestMethod]
        public void ConvertPentaMixedExpression_WithParanthesis()
        {
            const string originalExpression =
                "Id eq 456 and Tags/any(t: t eq 'Dummy') and (Details/Description eq 'Desc' or OrderLines/all(o: o/Name eq 'order')) and Name eq 'Dummy'";
            const string expectedExpression =
                "((ProductId eq 456 and ProductTags/any(t: t eq 'Dummy')) and (DetailsDescription eq 'Desc' or OrderLines/all(o: o eq 'order'))) and Name eq 'Dummy'";

            var actualConvertedExpression = _odataConverter.Convert(originalExpression);
            Assert.AreEqual(expectedExpression, actualConvertedExpression);
        }
        #endregion

        #region Negative Tests
        [ExpectedException(typeof(ODataException))]
        [TestMethod]
        public void ShouldExpectException_WhenOriginalExpressionHasWrongFieldName()
        {
            const string originalExpression = 
                "ProductId eq 456 and Tags/any(t: t eq 'Dummy') and Details/Description eq 'Desc'";
            _odataConverter.Convert(originalExpression);
        }

        [ExpectedException(typeof(ODataException))]
        [TestMethod]
        public void ShouldExpectException_WhenOriginalExpressionHasWrongFieldNames()
        {
            const string originalExpression =
                "Id eq 456 and ProductTags/any(t: t eq 'Dummy') and (Details/Description eq 'Desc' or OrderLine/all(o: o/Name eq 'order')) and Name eq 'Dummy'";
            _odataConverter.Convert(originalExpression);
        }

        [ExpectedException(typeof(ODataException))]
        [TestMethod]
        public void ShouldExpectException_WhenConvertedExpressionHasWrongFieldName()
        {
            const string originalExpression =
                "Id eq 456 and Tags/any(t: t eq 'Dummy') and Details/Description eq 'Desc'";

            var fakeFieldMapper = new StubIFieldMapper()
            {
                MapStringStringString = (productPropertyName, propertyParentName, root) =>
                {
                    if (productPropertyName == "Id")
                        return "Id";
                    if (productPropertyName == "Tags" && string.IsNullOrEmpty(propertyParentName))
                        return "Tags";
                    if (productPropertyName == "Description" && propertyParentName == "Details")
                        return "DetailsDescription";
                    return "";
                }
            };
            var productEdmModel = Product.GetEdmModel();
            var productDocsEdmModel = ProductDoc.GetProductsModel();
            _odataConverter = new ODataConverter(productEdmModel, typeof(Product), productDocsEdmModel, typeof(ProductDoc), fakeFieldMapper);
            var convertedExpression = _odataConverter.Convert(originalExpression);
        }
        #endregion
    }
}
