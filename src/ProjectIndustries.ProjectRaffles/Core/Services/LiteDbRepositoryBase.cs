using DynamicData;
using LiteDB;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public abstract class LiteDbRepositoryBase<T> : IRepository<T>
    where T : IEntity
  {
    protected readonly ISourceCache<T, Guid> Cache = new SourceCache<T, Guid>(_ => _.Id);
    protected readonly ILiteCollection<T> Collection;

    protected LiteDbRepositoryBase(ILiteDatabase database)
    {
      Collection = database.GetCollection<T>();
      Items = Cache.AsObservableCache();
    }

    public Task InitializeAsync(CancellationToken ct = default)
    {
      Cache.Clear();
      var items = Collection.FindAll();
      Cache.AddOrUpdate(items);

      return Task.CompletedTask;
    }

    public IObservableCache<T, Guid> Items { get; }

    public virtual Task SaveAsync(T item, CancellationToken ct = default)
    {
      Collection.Upsert(item);
      Cache.AddOrUpdate(item);
      return Task.CompletedTask;
    }

    public Task SaveSilentlyAsync(T item, CancellationToken ct = default)
    {
      Collection.Upsert(item);
      return Task.CompletedTask;
    }

    public Task SaveAsync(IEnumerable<T> items, CancellationToken ct = default)
    {
      var toSave = items.ToList();
      Collection.Upsert(toSave);
      Cache.AddOrUpdate(toSave);

      return Task.CompletedTask;
    }

    public Task RemoveAsync(T toRemove, CancellationToken ct = default)
    {
      Cache.Remove(toRemove);
      Collection.Delete(new BsonValue(toRemove.Id));
      return Task.CompletedTask;
    }

    public IEnumerable<T> LocalItems => Cache.Items;

    public async Task<IList<T>> GetAllAsync(CancellationToken ct = default)
    {
      if (Cache.Count == 0)
      {
        await InitializeAsync(ct);
      }

      return LocalItems.ToList();
    }
  }
}