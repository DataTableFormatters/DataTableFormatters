namespace DataTableFormatters
{
    public class NullRepresentation : IFormatConfiguration
    {
        public NullRepresentation(string nullText)
        {
            NullText = nullText;
        }

        public string NullText { get; }
    }
}