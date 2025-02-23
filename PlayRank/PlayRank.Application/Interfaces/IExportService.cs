using PlayRank.Application.Core.Interfaces.Abstract;

namespace PlayRank.Application.Core.Interfaces
{
    public interface IExportService : IService
    {
        string BuildCsv<T>(IEnumerable<T> data, Dictionary<string, Func<T, string>> columns);

        string EscapeCsv(string field);
    }
}
