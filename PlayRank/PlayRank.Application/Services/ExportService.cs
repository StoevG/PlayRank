using System.Text;
using PlayRank.Application.Core.Interfaces;

namespace PlayRank.Application.Core.Services
{
    public class ExportService : IExportService
    {
        public string BuildCsv<T>(IEnumerable<T> data, Dictionary<string, Func<T, string>> columns)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",", columns.Keys.Select(EscapeCsv)));
            foreach (var item in data)
            {
                var row = string.Join(",", columns.Values.Select(func => EscapeCsv(func(item))));
                sb.AppendLine(row);
            }
            return sb.ToString();
        }

        public string EscapeCsv(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";
            if (field.Contains(",") || field.Contains("\""))
            {
                field = field.Replace("\"", "\"\"");
                return $"\"{field}\"";
            }
            return field;
        }
    }
}
