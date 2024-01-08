using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public interface ICountriesService
  {
    Task InitializeAsync(CancellationToken ct = default);
    IReadOnlyList<Country> Countries { get; }
    public string GetCountryName(string id) => Countries.FirstOrDefault(_ => _.Id == id)?.Title;
    public string GetProvinceName(string countryId, string provinceId)
    {
      var country = Countries.FirstOrDefault(_ => _.Id == countryId);
      return country?.Provinces.FirstOrDefault(_ => _.Code == provinceId)?.Title;
    }
  }
}