using ODataSample.Interfaces;

namespace ODataSample.Services
{
    public class ProductFieldMapper: IFieldMapper
    {
        public string Map(string productPropertyName, string propertyParentName)
        {
            //TODO - Remove property naming and use OData references
            if (productPropertyName == "Id") //Bad Practice - Not considering the name
                return "ProductId";
            if (productPropertyName == "Name" && string.IsNullOrEmpty(propertyParentName))
                return "Name";
            if (productPropertyName == "Tags" && string.IsNullOrEmpty(propertyParentName))
                return "ProductTags";
            if (productPropertyName == "Description" && propertyParentName == "Details")
                return "DetailsDescription";
            if (productPropertyName == "Description" && string.IsNullOrEmpty(propertyParentName))
                return "ShortDescription";
            if (productPropertyName == "Name" && (propertyParentName == "Language" || propertyParentName == "Lang"))
                return "DescriptionLanguageName";
            if (productPropertyName == "Name" && (propertyParentName == "Reference" || propertyParentName == "References"))
                return "DetailReferenceNames";

            return "";
        }
    }
}