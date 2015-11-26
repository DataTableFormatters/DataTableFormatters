using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataTableFormatters
{
    internal class MonospacedDataTableFormatter : BaseDataTableFormatter
    {
        private readonly IFormatConfiguration[] _options;

        public MonospacedDataTableFormatter(IFormatConfiguration[] options)
        {
            _options = options;
        }

        private const string UpperLeftCorner = "+";
        private const string UpperBorder = "-";
        private const string UpperSplit = "+";
        private const string UpperRightCorner = "+";

        private const string MiddleLeftCorner = "+";
        private const string MiddleBorder = "-";
        private const string MiddleSplit = "+";
        private const string MiddleRightCorner = "+";

        private const string LowerLeftCorner = "+";
        private const string LowerBorder = "-";
        private const string LowerSplit = "+";
        private const string LowerRightCorner = "+";

        private const string LeftBorder = "|";
        private const string Split = "|";
        private const string RightBorder = "|";

        protected override IEnumerable<string> GetStringRepresentation(DataTable dataTable)
        {
            if (dataTable == null)
                throw new ArgumentNullException("dataTable");

            var columnWidths = dataTable.DataColumns().Select(ColumnMaxElementLength).ToList();

            return Concatenate(
                UpperHorizontalLine(columnWidths),
                Headers(columnWidths),
                MiddleHorizontalLine(columnWidths),
                Contents(dataTable.DataRows(), columnWidths),
                LowerHorizontalLine(columnWidths));
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

        private static IEnumerable<string> UpperHorizontalLine(IList<ColumnAndWidth> columnWidths)
        {
            return Interlace(
                UpperLeftCorner + UpperBorder,
                columnWidths.Select(x => new string(UpperBorder.Single(), x.Width)),
                UpperBorder + UpperSplit + UpperBorder,
                UpperBorder + UpperRightCorner + Environment.NewLine);
        }

        private static IEnumerable<string> MiddleHorizontalLine(IList<ColumnAndWidth> columnWidths)
        {
            return Interlace(
                MiddleLeftCorner + MiddleBorder,
                columnWidths.Select(x => new string(MiddleBorder.Single(), x.Width)),
                MiddleBorder + MiddleSplit + MiddleBorder,
                MiddleBorder + MiddleRightCorner + Environment.NewLine);
        }

        private static IEnumerable<string> LowerHorizontalLine(IList<ColumnAndWidth> columnWidths)
        {
            return Interlace(
                LowerLeftCorner + LowerBorder,
                columnWidths.Select(x => new string(LowerBorder.Single(), x.Width)),
                LowerBorder + LowerSplit + LowerBorder,
                LowerBorder + LowerRightCorner + Environment.NewLine);
        }

        private IEnumerable<string> Headers(IList<ColumnAndWidth> columnWidths)
        {
            return Interlace(
                LeftBorder + " ",
                columnWidths.Select(x =>
                {
                    var alignedColumnName = Align(x.ColumnName, x.ColumnName, x.Width);
                    return alignedColumnName;
                }),
                " " + Split + " ",
                " " + RightBorder + Environment.NewLine);
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
                LeftBorder + " ",
                columnWidths.Select((x, i) => Cell(row, i, x)),
                " " + Split + " ",
                " " + RightBorder + Environment.NewLine);
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
                var format = _options.OfType<CustomFormat>().FirstOrDefault(y => y.ColumnName == columnWidth.ColumnName);
                var formatString = format == null ? "{0}" : "{0:" + format.FormatString + "}";
                stringRepresentation = string.Format(formatString, row[index]);
            }

            string alignedContent = Align(columnWidth.ColumnName, stringRepresentation, columnWidth.Width);
            return alignedContent;
        }

        private static IEnumerable<T> Interlace<T>(T prefix, IEnumerable<T> list, T separator, T suffix)
        {
            yield return prefix;
            if (list.Any())
            {
                yield return list.First();
                foreach (T item in list.Skip(1))
                {
                    yield return separator;
                    yield return item;
                }
            }

            yield return suffix;
        }

        private static IEnumerable<T> Concatenate<T>(params IEnumerable<T>[] lists)
        {
            return lists.SelectMany(x => x);
        }

        private class ColumnAndWidth
        {
            public ColumnAndWidth(DataColumn column, int width)
            {
                ColumnName = column.ColumnName;
                Width = width;
            }

            public string ColumnName { get; set; }
            public int Width { get; set; }
        }
    }
}