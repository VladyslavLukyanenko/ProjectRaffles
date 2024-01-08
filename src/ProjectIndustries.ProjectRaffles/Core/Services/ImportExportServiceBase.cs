using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public abstract class ImportExportServiceBase<T> : IImportExportService
    where T : IEntity
  {
    protected readonly IRepository<T> Repository;

    protected ImportExportServiceBase(IRepository<T> repository)
    {
      Repository = repository;
    }

    public async Task ExportAsJsonAsync(Stream output, CancellationToken ct)
    {
      var lists = await Repository.GetAllAsync(ct);
      var json = JsonConvert.SerializeObject(lists);
      await using var writer = new StreamWriter(output);
      await writer.WriteAsync(json);
    }

    public async Task<bool> ImportFromJsonAsync(Stream input, CancellationToken ct)
    {
      using var reader = new StreamReader(input);
      var json = await reader.ReadToEndAsync();
      var lists = JsonConvert.DeserializeObject<List<T>>(json);
      await Repository.SaveAsync(lists, ct);
      return true;
    }

    public abstract Task ExportAsCsvAsync(Stream output, CancellationToken ct);
    public abstract Task<bool> ImportFromCsvAsync(Stream input, CancellationToken ct);

    protected async Task<IList<TData>> ReadRecordsFromCsvAsync<TData>(Stream stream, CancellationToken ct = default)
    {
      using var reader = new StreamReader(stream);
      using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
      return await csv.GetRecordsAsync<TData>().ToListAsync(ct);
    }

    protected async Task WriteRecordsToCsvAsync<TData>(Stream output, IEnumerable<TData> data)
    {
      await using var writer = new StreamWriter(output);
      await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
      await csv.WriteRecordsAsync(data);
    }
  }
}