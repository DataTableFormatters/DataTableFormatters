using System.Data;

namespace DataTableFormatters
{
    internal interface IDataTableFormatter
    {
        string GetStringRepresentation(DataTable dataTable);
    }
}