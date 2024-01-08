using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Clients;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public class CountriesService
    : ICountriesService
  {
    private readonly ILicenseKeyProvider _licenseKeyProvider;
    private readonly ICountriesApiClient _countriesApiClient;

    public CountriesService(ILicenseKeyProvider licenseKeyProvider, ICountriesApiClient countriesApiClient)
    {
      _licenseKeyProvider = licenseKeyProvider;
      _countriesApiClient = countriesApiClient;
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
      var countries = await _countriesApiClient.GetCountriesAsync(_licenseKeyProvider.CurrentLicenseKey, ct);
      Countries = countries.ToList();
    }

    public IReadOnlyList<Country> Countries { get; private set; } = new List<Country>();
  }
}