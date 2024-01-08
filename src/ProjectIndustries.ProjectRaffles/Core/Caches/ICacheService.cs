using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Caches
{
  public interface ICacheService
  {
    Task<CacheEntry> GetEntryAsync(string key, CancellationToken ct = default);
    Task<object> GetAsync(string key, Type resultType, CancellationToken ct = default);
    Task<T> GetAsync<T>(string key, CancellationToken ct = default);

    Task<CacheEntry> SetAsync(string key, object payload, CancellationToken ct = default);
    Task<CacheEntry> SetAsync<T>(string key, T payload, CancellationToken ct = default);
  }
}