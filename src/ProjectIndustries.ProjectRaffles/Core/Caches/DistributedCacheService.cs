using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Clients;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Caches
{
  public class DistributedCacheService : ApiClientBase, ICacheService
  {
    private const string BaseRelativeUrlPattern = "/shared-cache?key={0}";
    private readonly ILicenseKeyProvider _licenseKeyProvider;

    public DistributedCacheService(ProjectIndustriesApiConfig apiConfig, ILicenseKeyProvider licenseKeyProvider)
      : base(apiConfig)
    {
      _licenseKeyProvider = licenseKeyProvider;
    }

    public Task<CacheEntry> GetEntryAsync(string key, CancellationToken ct = default)
    {
      var url = BuildRelativeUrl(key);
      return GetAsync<CacheEntry>(url, _licenseKeyProvider.CurrentLicenseKey, ct);
    }

    public async Task<object> GetAsync(string key, Type resultType, CancellationToken ct = default)
    {
      var entry = await GetEntryAsync(key, ct);
      if (entry == null)
      {
        return resultType.IsValueType ? Activator.CreateInstance(resultType) : null;
      }

      var result = JsonConvert.DeserializeObject(entry.Payload, resultType);

      return result;
    }

    public async Task<T> GetAsync<T>(string key, CancellationToken ct = default)
    {
      return (T) await GetAsync(key, typeof(T), ct);
    }

    public async Task<CacheEntry> SetAsync(string key, object payload, CancellationToken ct = default)
    {
      var url = BuildRelativeUrl(key);
      var entry = new CacheEntry
      {
        Payload = JsonConvert.SerializeObject(payload)
      };

      return await PostAsync<CacheEntry>(url, _licenseKeyProvider.CurrentLicenseKey, entry, ct);
    }

    private static string BuildRelativeUrl(string key)
    {
      return string.Format(BaseRelativeUrlPattern, Uri.EscapeUriString(key));
    }

    public Task<CacheEntry> SetAsync<T>(string key, T payload, CancellationToken ct = default)
    {
      return SetAsync(key, (object) payload, ct);
    }
  }
}