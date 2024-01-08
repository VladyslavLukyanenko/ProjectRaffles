using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public interface IRaffleModule : IDisposable
  {
    string Name { get; }

    IObservable<string> ComputedIdentityChanged { get; }
    string ComputedIdentity { get; }

    Product TargetProduct { get; }
    void SetHttpClientBuilder(IHttpClientBuilder builder);
    Task InitializeAsync(IRaffleExecutionContext context, CancellationToken ct);
    Task PrepareAsync(CancellationToken ct);
    void SetFields(IDictionary<string, object> participateArguments);
    void InitializeFromPrototype(IRaffleModule module);
    IEnumerable<Field> AdditionalFields { get; }

    RaffleStatus Status { get; }
    event EventHandler StatusChanged;
    // event EventHandler<RaffleModuleExecutingEventArgs> Executing;
    event EventHandler AdditionalFieldsChanged;

    Task ExecuteAsync(IRaffleExecutionContext context, CancellationToken ct);
  }
}