using System.Web.Http.OData.Query;

namespace ODataSample.Interfaces
{
    public interface IODataExpressionConverter
    {
        string Convert(string originalExpression);
        FilterQueryOption Convert(FilterQueryOption originalExpression);
    }
}