namespace DataTableFormatters
{
    public class CustomFormat : IFormatConfiguration
    {
        public CustomFormat(string columnName, string formatString)
        {
            ColumnName = columnName;
            FormatString = formatString;
        }

        public string ColumnName { get; set; }

        public string FormatString { get; set; }
    }
}