using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Proxies
{
  public class ProxyImportExportService : ImportExportServiceBase<ProxyGroup>, IProxyImportExportService
  {
    private readonly IMapper _mapper;

    public ProxyImportExportService(IProxyGroupsRepository repository, IMapper mapper)
      : base(repository)
    {
      _mapper = mapper;
    }

    public override async Task ExportAsCsvAsync(Stream output, CancellationToken ct)
    {
      var groups = await Repository.GetAllAsync(ct);
      var records = new List<CsvProxyData>(groups.Sum(_ => _.Proxies.Count));
      foreach (var proxyGroup in groups)
      {
        var mapped = _mapper.Map<List<CsvProxyData>>(proxyGroup.Proxies);
        foreach (var proxyData in mapped)
        {
          proxyData.GroupName = proxyGroup.Name;
        }

        records.AddRange(mapped);
      }

      await WriteRecordsToCsvAsync(output, records);
    }

    public override async Task<bool> ImportFromCsvAsync(Stream input, CancellationToken ct)
    {
      IList<CsvProxyData> data = await ReadRecordsFromCsvAsync<CsvProxyData>(input, ct);
      var groups = data.GroupBy(_ => _.GroupName)
        .Select(group =>
        {
          var proxies = _mapper.Map<IList<Proxy>>(group);
          return new ProxyGroup(group.Key, proxies);
        });

      await Repository.SaveAsync(groups, ct);
      return true;
    }

  }
}