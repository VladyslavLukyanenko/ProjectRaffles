using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Clients
{
  public interface ICountriesApiClient
  {
    Task<IList<Country>> GetCountriesAsync(string licenseKey, CancellationToken ct = default);
  }
}