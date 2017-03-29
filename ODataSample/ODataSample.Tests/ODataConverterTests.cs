using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ODataSample.Interfaces;
using ODataSample.Services;
using ODataSample.Models;
using ODataSample.Models.EDM;

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
            var productDocsEdmModel = ProductDocEdm.GetProductsModel();
            _odataConverter = new ODataConverter(productEdmModel, typeof(Product), productDocsEdmModel, typeof(ProductDocEdm), _fieldMapper);
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
    }
}
