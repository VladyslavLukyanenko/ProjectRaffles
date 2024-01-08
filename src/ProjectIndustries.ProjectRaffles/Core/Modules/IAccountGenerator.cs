using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public interface IAccountGenerator
  {
    IEnumerable<Field> ConfigurationFields { get; }
    IAsyncEnumerable<Account> GenerateAsync(CancellationToken ct = default);
    Task InitializeAsync(Func<Profile> profileProvider, CancellationToken ct = default);
    Task PrepareAsync(CancellationToken ct = default);
  }
}