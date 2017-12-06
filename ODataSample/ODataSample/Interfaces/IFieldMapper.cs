namespace ODataSample.Interfaces
{
    public interface IFieldMapper
    {
        string Map(string propertyName, string parentPropertyName, string rootPath);
    }
}