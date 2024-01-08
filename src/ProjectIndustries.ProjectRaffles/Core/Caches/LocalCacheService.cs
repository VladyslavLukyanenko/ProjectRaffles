using System;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Caches
{
  public class LocalCacheService : ICacheService
  {
    private readonly ILiteCollection<LocalCacheEntry> _cacheEntries;
    private readonly ICacheService _fallbackImpl;

    public LocalCacheService(ILiteDatabase database, ICacheService fallbackImpl)
    {
      _fallbackImpl = fallbackImpl;
      _cacheEntries = database.GetCollection<LocalCacheEntry>();
      _cacheEntries.EnsureIndex(_ => _.ExpiresAt);
    }

    public async Task<CacheEntry> GetEntryAsync(string key, CancellationToken ct = default)
    {
      var id = new BsonValue(key);
      var entry = _cacheEntries.FindById(id);
      if (entry == null)
      {
        var e = await _fallbackImpl.GetEntryAsync(key, ct);
        if (e == null)
        {
          return null;
        }

        Upsert(key, e);

        return e;
      }

      if (entry.IsExpired())
      {
        return null;
      }

      return new CacheEntry
      {
        Payload = entry.Payload,
        ExpiresAt = entry.ExpiresAt.ToUnixTimeSeconds()
      };
    }

    public async Task<object> GetAsync(string key, Type resultType, CancellationToken ct = default)
    {
      var e = await GetEntryAsync(key, ct);

      if (e == null)
      {
        return GetDefaultValue(resultType);
      }

      return JsonConvert.DeserializeObject(e.Payload, resultType);
    }

    public async Task<T> GetAsync<T>(string key, CancellationToken ct = default)
    {
      var r = await GetAsync(key, typeof(T), ct);
      return (T) r;
    }

    public async Task<CacheEntry> SetAsync(string key, object payload, CancellationToken ct = default)
    {
      var e = await _fallbackImpl.SetAsync(key, payload, ct);
      Upsert(key, e);

      return e;
    }

    public Task<CacheEntry> SetAsync<T>(string key, T payload, CancellationToken ct = default)
    {
      return SetAsync(key, (object) payload, ct);
    }

    private void Upsert(string key, CacheEntry e)
    {
      _cacheEntries.Upsert(key, new LocalCacheEntry
      {
        Payload = e.Payload,
        CreatedAt = DateTimeOffset.UtcNow,
        ExpiresAt = DateTimeOffset.FromUnixTimeSeconds(e.ExpiresAt)
      });
    }

    private static object GetDefaultValue(Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;
  }
}