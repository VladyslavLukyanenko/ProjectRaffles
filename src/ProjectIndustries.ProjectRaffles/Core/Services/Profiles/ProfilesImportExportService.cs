using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Profile = ProjectIndustries.ProjectRaffles.Core.Domain.Profile;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Profiles
{
  public class ProfilesImportExportService : ImportExportServiceBase<Profile>, IProfilesImportExportService
  {
    private readonly IMapper _mapper;

    public ProfilesImportExportService(IProfilesRepository repository, IMapper mapper)
      : base(repository)
    {
      _mapper = mapper;
    }

    public override async Task ExportAsCsvAsync(Stream output, CancellationToken ct)
    {
      var profiles = await Repository.GetAllAsync(ct);
      var data = _mapper.Map<List<CsvProfileData>>(profiles);
      await WriteRecordsToCsvAsync(output, data);
    }

    public override async Task<bool> ImportFromCsvAsync(Stream input, CancellationToken ct)
    {
      var records = await ReadRecordsFromCsvAsync<CsvProfileData>(input, ct);
      var profiles = _mapper.Map<List<Profile>>(records);
      await Repository.SaveAsync(profiles, ct);

      return true;
    }
  }
}