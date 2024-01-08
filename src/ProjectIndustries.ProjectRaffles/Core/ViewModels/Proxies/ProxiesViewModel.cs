using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Proxies;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Proxies
{
  public class ProxiesViewModel
    : PageViewModelBase, IRoutableViewModel
  {
    private readonly IProxyGroupsRepository _proxyGroupsRepository;
    private readonly IToastNotificationManager _toasts;
    private readonly ProxiesHeaderViewModel _header;
    private readonly ReadOnlyObservableCollection<ProxyGroup> _proxyGroups;

    public ProxiesViewModel(IProxyGroupsRepository proxyGroupsRepository, IScreen hostScreen, IMessageBus messageBus,
      IToastNotificationManager toasts, ProxiesHeaderViewModel header)
      : base("Proxies", messageBus)
    {
      _proxyGroupsRepository = proxyGroupsRepository;
      _toasts = toasts;
      _header = header;
      HostScreen = hostScreen;
      var items = proxyGroupsRepository.Items.Connect();
      items
        .Bind(out _proxyGroups)
        .DisposeMany()
        .Subscribe();

      items.ToCollection()
        .Where(i => i.Any())
        .Subscribe(i =>
        {
          if (SelectedProxyGroup == null)
          {
            SelectedProxyGroup = i.First();
          }
        });

      var canCreate = this.WhenAnyValue(_ => _.NewGroupName)
        .CombineLatest(this.WhenAnyValue(_ => _.RawProxies),
          (grp, prx) => !string.IsNullOrWhiteSpace(grp) && !string.IsNullOrWhiteSpace(prx));

      var proxyGroupStream = this.WhenAnyValue(_ => _.SelectedProxyGroup);
      var canRemove = proxyGroupStream
        .Select(g => g != null);

      CreateCommand = ReactiveCommand.CreateFromTask(CreateProxiesAsync, canCreate);
      CreateCommand.IsExecuting.ToPropertyEx(this, _ => _.IsCreating);

      RemoveGroupCommand = ReactiveCommand.CreateFromTask(async ct =>
      {
        await proxyGroupsRepository.RemoveAsync(SelectedProxyGroup, ct);
        toasts.Show(ToastContent.Success("Group removed successfully"));
      }, canRemove);

      IDisposable proxyGroupChanged = null;
      // todo: create some kind of list which will do it in his side. The same thing on accounts grid
      proxyGroupStream.Subscribe(g => { proxyGroupChanged = RefreshProxies(proxyGroupChanged, g); });
    }

    private IDisposable RefreshProxies(IDisposable proxyGroupChanged, ProxyGroup g)
    {
      ProxyRows.Clear();
      proxyGroupChanged?.Dispose();
      if (g == null)
      {
        return proxyGroupChanged;
      }

      AddAllRows();

      proxyGroupChanged = g.Proxies.ObserveCollectionChanges()
        .Subscribe(_ =>
        {
          ProxyRows.Clear();
          AddAllRows();
        });


      void AddAllRows()
      {
        using (ProxyRows.SuspendNotifications())
        {
          ProxyRows.AddRange(g.Proxies.Select(a => new ProxyRowViewModel(a, g, _proxyGroupsRepository, _toasts)));
        }
      }

      return proxyGroupChanged;
    }

    private async Task CreateProxiesAsync(CancellationToken ct)
    {
      if (string.IsNullOrEmpty(RawProxies))
      {
        return;
      }

      var validProxies = new List<Proxy>();
      var invalidProxies = new List<string>();
      var tokens = RawProxies.Split(new[] {"\n", Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
      foreach (var raw in tokens)
      {
        await CreateProxyAsync(raw, validProxies, invalidProxies, ct);
      }

      if (invalidProxies.Any())
      {
        _toasts.Show(ToastContent.Error("Some of proxies are malformed or unavailable. They were skipped.",
          "Can't save all proxies"));

        // _toasts.Show(ToastContent.Error(
        //   "Some of proxies are malformed. Please check them and try again", "Can't save all proxies"));
        RawProxies = string.Join(Environment.NewLine, invalidProxies);
      }
      else
      {
        _toasts.Show(ToastContent.Success("Proxies are saved."));
        RawProxies = null;
      }

      if (!validProxies.Any())
      {
        _toasts.Show(ToastContent.Error($"No valid proxies. They maybe are malformed or unavailable"));
        return;
      }


      var group = new ProxyGroup(NewGroupName, validProxies);
      await _proxyGroupsRepository.SaveAsync(group, ct);
      SelectedProxyGroup = group;
      NewGroupName = null;
      SkipAvailabilityCheck = false;
    }

    private async Task CreateProxyAsync(string raw, List<Proxy> validProxies, List<string> invalidProxies,
      CancellationToken ct)
    {
      if (Proxy.TryParse(raw, out var proxy) && (SkipAvailabilityCheck || await IsProxyAvailableAsync(proxy, ct)))
      {
        validProxies.Add(proxy);
        if (!SkipAvailabilityCheck)
        {
          await Task.Delay(TimeSpan.FromMilliseconds(25), ct);
        }
      }
      else
      {
        invalidProxies.Add(raw);
      }
    }

    private async Task<bool> IsProxyAvailableAsync(Proxy proxy, CancellationToken ct)
    {
      var client = new HttpClient(new HttpClientHandler
      {
        Proxy = proxy.ToWebProxy(),
        UseProxy = true
      });

      var message = new HttpRequestMessage(HttpMethod.Get, "https://api.projectindustries.gg/product/project-raffles");
      try
      {
        var r = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, ct);
        return r.IsSuccessStatusCode;
      }
      catch
      {
        return false;
      }
    }

    protected override ViewModelBase GetHeaderContent() => _header;

    public ObservableCollectionExtended<ProxyRowViewModel> ProxyRows { get; } =
      new ObservableCollectionExtended<ProxyRowViewModel>();

    public ReadOnlyObservableCollection<ProxyGroup> ProxyGroups => _proxyGroups;
    [Reactive] public ProxyGroup SelectedProxyGroup { get; set; }
    [Reactive] public string NewGroupName { get; set; }
    [Reactive] public string RawProxies { get; set; }
    [Reactive] public bool SkipAvailabilityCheck { get; set; }
    public string UrlPathSegment => nameof(ProxiesViewModel);
    public IScreen HostScreen { get; }
    public bool IsCreating { [ObservableAsProperty] get; }

    public ReactiveCommand<Unit, Unit> CreateCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> RemoveGroupCommand { get; private set; }
  }
}