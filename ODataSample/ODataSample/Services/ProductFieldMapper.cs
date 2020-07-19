using ODataSample.Interfaces;

namespace ODataSample.Services
{
    public class ProductFieldMapper: IFieldMapper
    {
        public string Map(string productPropertyName, string propertyParentName, string rootProperyPath)
        {
            //TODO - Remove property naming and use OData references
            if (productPropertyName == "Id") //Bad Practice - Not considering the parent name
                return "ProductId";
            if (productPropertyName == "Name" && string.IsNullOrEmpty(propertyParentName))
                return "Name";
            if (productPropertyName == "Tags" && string.IsNullOrEmpty(propertyParentName))
                return "ProductTags";
            if (productPropertyName == "Description" && propertyParentName == "Details")
                return "DetailsDescription";
            if (productPropertyName == "Description" && string.IsNullOrEmpty(propertyParentName))
                return "ShortDescription";
            if (productPropertyName == "Name" && propertyParentName == "Lang" && rootProperyPath == "Details")
                return "DescriptionLanguageName";
            if (productPropertyName == "Name" && propertyParentName == "References" && rootProperyPath == "Details")
                return "DetailReferenceNames";
            if (productPropertyName == "Name" && propertyParentName == "OrderLines")
                return "OrderLines";

            return "";
        }
    }
}