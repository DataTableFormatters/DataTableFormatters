using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataTableFormatters
{
    internal abstract class BaseDataTableFormatter : IDataTableFormatter
    {
        string IDataTableFormatter.GetStringRepresentation(DataTable dataTable)
        {
            return string.Join("", GetStringRepresentation(dataTable));
        }

        protected abstract IEnumerable<string> GetStringRepresentation(DataTable dataTable);

        protected static IEnumerable<T> Interlace<T>(T prefix, IEnumerable<T> list, T separator, T suffix)
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

        protected static IEnumerable<T> Concatenate<T>(params IEnumerable<T>[] lists)
        {
            return lists.SelectMany(x => x);
        }

    }
}