using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public interface IRepository<T>
    where T : IEntity
  {
    Task InitializeAsync(CancellationToken ct = default);
    IObservableCache<T, Guid> Items { get; }
    Task SaveAsync(T item, CancellationToken ct = default);
    Task SaveSilentlyAsync(T item, CancellationToken ct = default);
    Task SaveAsync(IEnumerable<T> items, CancellationToken ct = default);
    Task RemoveAsync(T toRemove, CancellationToken ct = default);
    IEnumerable<T> LocalItems { get; }
    Task<IList<T>> GetAllAsync(CancellationToken ct = default);
  }
}