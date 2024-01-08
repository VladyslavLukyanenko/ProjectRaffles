using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Modules;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Modules
{
  public class RaffleTask : ViewModelBase, IRaffleExecutionContext
  {
    private readonly IHttpClientBuilder _builder;

    public RaffleTask(long id, IRaffleModule module, Proxy proxy, string proxyGroupName, Profile profile,
      IHttpClientBuilder builder, IReadonlyDependencyResolver dependencyResolver)
    {
      _builder = builder;
      DependencyResolver = dependencyResolver;
      Id = id;
      Module = module;
      Proxy = proxy;
      ProxyGroupName = proxyGroupName;
      Profile = profile;

      ProviderName = module.Name;
      Module.StatusChanged += ModuleOnStatusChanged;
    }

    private void ModuleOnStatusChanged(object sender, EventArgs e)
    {
      Status = Module.Status;
    }

    public async Task InitializeAsync(CancellationToken ct)
    {
      await Module.InitializeAsync(this, ct);
    }

    public async Task PrepareAsync(CancellationToken ct)
    {
      try
      {
        if (Proxy != null)
        {
          _builder.WithProxy(Proxy.ToWebProxy());
        }

        Module.SetHttpClientBuilder(_builder);
        await Module.PrepareAsync(ct);
        ProductName = Module.TargetProduct.Name;
      }
      catch (Exception exc)
      {
        UnknownError(exc.Message);
      }
    }

    public long Id { get; private set; }

    public string ProviderName { get; set; }
    public string Size { get; } = "Random";
    public Profile Profile { get; set; }
    public IRaffleModule Module { get; set; }

    public Proxy Proxy { get; }
    public IReadonlyDependencyResolver DependencyResolver { get; }

    // public Email Email { get; private set; }
    public string ProxyGroupName { get; }

    [Reactive] public string ProductName { get; set; }
    [Reactive] public RaffleStatus Status { get; private set; } = RaffleStatus.Created;

    public void Cancel()
    {
      Status = RaffleStatus.Cancelled;
    }

    public void FailedWithCause(string message, string rootCause)
    {
      Status = RaffleStatus.FailedWithCause(message, rootCause);
    }

    public void UnknownError(string message)
    {
      Status = RaffleStatus.UnknownError(message);
    }

    public async Task DelayBeforeProcessing(TimeSpan delay, CancellationToken ct)
    {
      Status = RaffleStatus.DelayBeforeProcessing;
      await Task.Delay(delay, ct);
    }

    public void Schedule()
    {
      Status = RaffleStatus.Scheduled;
    }
  }
}