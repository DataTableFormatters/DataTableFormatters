using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataTableFormatters
{
    internal class MarkupDataTableFormatter : BaseDataTableFormatter
    {
        private readonly IFormatConfiguration[] _options;

        public MarkupDataTableFormatter(IFormatConfiguration[] options)
        {
            _options = options;
        }

        protected override IEnumerable<string> GetStringRepresentation(DataTable dataTable)
        {
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable));

            var columnWidths = dataTable.DataColumns().Select(ColumnMaxElementLength).ToList();

            return Concatenate(
                Headers(columnWidths),
                Contents(dataTable.DataRows(), columnWidths));
        }

        private static ColumnAndWidth ColumnMaxElementLength(DataColumn column)
        {
            int? maxLength = column.Table.DataRows()
                .Select(row => (int?)row[column].ToString().Length)
                .Max();

            return new ColumnAndWidth(
                column,
                Math.Max(
                    column.ColumnName.Length, 
                    maxLength ?? 0));
        }

        private IEnumerable<string> Headers(IList<ColumnAndWidth> columnWidths)
        {
            return Interlace(
                "<thead><tr><th>",
                columnWidths.Select(x =>
                {
                    var alignedColumnName = Align(x.ColumnName, x.ColumnName, x.Width);
                    return alignedColumnName;
                }),
                "</th><th>",
                "</th></tr></thead>");
        }

        private string Align(string columnName, string content, int width)
        {
            return _options.OfType<RightAlign>().Any(x => x.ColumnName == columnName) 
                ? content.PadLeft(width) 
                : content.PadRight(width);
        }

        private IEnumerable<string> Contents(IEnumerable<DataRow> rows, IList<ColumnAndWidth> columnWidths)
        {
            return rows.SelectMany(row => Row(columnWidths, row));
        }

        private IEnumerable<string> Row(IList<ColumnAndWidth> columnWidths, DataRow row)
        {
            return Interlace(
                "<tbody><tr><td>",
                columnWidths.Select((x, i) => Cell(row, i, x)),
                "</td><td>",
                "</td></tr></tbody>");
        }

        private string Cell(DataRow row, int index, ColumnAndWidth columnWidth)
        {
            string stringRepresentation;
            if (row[index] == DBNull.Value)
            {
                stringRepresentation = "NULL";
            }
            else
            {
                var format = _options.OfType<CustomColumnFormat>().FirstOrDefault(y => y.ColumnName == columnWidth.ColumnName);
                var formatString = format == null ? "{0}" : "{0:" + format.FormatString + "}";
                stringRepresentation = string.Format(formatString, row[index]);
            }

            string alignedContent = Align(columnWidth.ColumnName, stringRepresentation, columnWidth.Width);
            return alignedContent;
        }
        
        private class ColumnAndWidth
        {
            public ColumnAndWidth(DataColumn column, int width)
            {
                ColumnName = column.ColumnName;
                Width = width;
            }

            public string ColumnName { get; }
            public int Width { get; }
        }        
    }
}