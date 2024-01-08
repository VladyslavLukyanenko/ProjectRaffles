using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Accounts
{
  public class AccountImportExportService : ImportExportServiceBase<AccountGroup>, IAccountImportExportService
  {
    private readonly IMapper _mapper;
    public AccountImportExportService(IRepository<AccountGroup> repository, IMapper mapper)
      : base(repository)
    {
      _mapper = mapper;
    }

    public override async Task ExportAsCsvAsync(Stream output, CancellationToken ct)
    {
      var groups = await Repository.GetAllAsync(ct);
      var records = new List<CsvAccountData>(groups.Sum(_ => _.Accounts.Count));
      foreach (var accountGroup in groups)
      {
        var mapped = _mapper.Map<List<CsvAccountData>>(accountGroup.Accounts);
        foreach (var account in mapped)
        {
          account.GroupName = accountGroup.Name;
        }

        records.AddRange(mapped);
      }

      await WriteRecordsToCsvAsync(output, records);
    }

    public override async Task<bool> ImportFromCsvAsync(Stream input, CancellationToken ct)
    {
      IList<CsvAccountData> data = await ReadRecordsFromCsvAsync<CsvAccountData>(input, ct);
      var groups = data.GroupBy(_ => _.GroupName)
        .Select(group =>
        {
          var accounts = _mapper.Map<IList<Account>>(group);
          return new AccountGroup(group.Key, accounts);
        });

      await Repository.SaveAsync(groups, ct);
      return true;
    }
  }
}