using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public interface IImportExportService
  {
    Task ExportAsCsvAsync(Stream output, CancellationToken ct);
    Task ExportAsJsonAsync(Stream output, CancellationToken ct);
    Task<bool> ImportFromJsonAsync(Stream input, CancellationToken ct);
    Task<bool> ImportFromCsvAsync(Stream input, CancellationToken ct);
  }
}