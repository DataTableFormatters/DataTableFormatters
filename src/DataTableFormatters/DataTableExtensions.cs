using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataTableFormatters
{
    public static class DataTableExtensions
    {
        public static string ToMySql(this DataTable dataTable, params IFormatConfiguration[] options)
        {
            IDataTableFormatter formatter = new MonospacedDataTableFormatter(options);
            return formatter.GetStringRepresentation(dataTable);
        }

        public static string ToHtml(this DataTable dataTable, params IFormatConfiguration[] options)
        {
            IDataTableFormatter formatter = new MarkupDataTableFormatter(options);
            return formatter.GetStringRepresentation(dataTable);
        }

        public static IEnumerable<DataColumn> DataColumns(this DataTable dataTable)
        {
            return dataTable.Columns.Cast<DataColumn>();
        }

        public static IEnumerable<DataRow> DataRows(this DataTable dataTable)
        {
            return dataTable.Rows.Cast<DataRow>();
        }
    }
}