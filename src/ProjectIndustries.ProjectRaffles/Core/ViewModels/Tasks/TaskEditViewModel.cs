using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;
using ProjectIndustries.ProjectRaffles.Core.Services.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Profiles;
using ProjectIndustries.ProjectRaffles.Core.Services.Proxies;
using ProjectIndustries.ProjectRaffles.Core.Services.Tasks;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Tasks
{
  public class TaskEditViewModel : ViewModelBase
  {
    private readonly IRaffleModulesProvider _modulesProvider;
    private readonly IRaffleModuleFactory _raffleModuleFactory;
    private readonly ITasksService _tasksService;
    private readonly IToastNotificationManager _toasts;
    private readonly IModuleUsageStatsService _usageStatsService;
    private readonly ICaptchaResolversProvider _captchaResolversProvider;
    private readonly ReadOnlyObservableCollection<TaskProfileViewModel> _profiles;
    private readonly ReadOnlyObservableCollection<TaskProxyViewModel> _proxyGroups;
    private IDictionary<string, object> _initialFieldValues;

    public TaskEditViewModel(IProfilesRepository profilesRepository, IProxyGroupsRepository proxyGroupsRepository,
      IRaffleModulesProvider modulesProvider, IRaffleModuleFactory raffleModuleFactory, ITasksService tasksService,
      IToastNotificationManager toasts, IModuleUsageStatsService usageStatsService,
      ICaptchaResolversProvider captchaResolversProvider)
    {
      _modulesProvider = modulesProvider;
      _raffleModuleFactory = raffleModuleFactory;
      _tasksService = tasksService;
      _toasts = toasts;
      _usageStatsService = usageStatsService;
      _captchaResolversProvider = captchaResolversProvider;

      SetupAvailableModulesChangeHandler();

      var profilesFilter = this.WhenAnyValue(_ => _.SelectedModuleDescriptor)
        .Select(BuildProfileFilter);
      var profilesSortComparer = SortExpressionComparer<TaskProfileViewModel>.Descending(_ => _.IsEnabled)
        .ThenByAscending(_ => _.Profile.ProfileName);
      var profilesStream = profilesRepository.Items.Connect()
        .Transform(p => new TaskProfileViewModel(p))
        .AutoRefresh(_ => _.IsEnabled)
        .Filter(profilesFilter)
        .Publish()
        .RefCount();

      this.WhenAnyValue(_ => _.SelectedModuleDescriptor)
        .CombineLatest(profilesStream.ToCollection(), (descriptor, profiles) => (descriptor, profiles))
        .Throttle(TimeSpan.FromMilliseconds(500))
        .ObserveOn(RxApp.MainThreadScheduler)
        .Subscribe(item =>
        {
          foreach (var profile in item.profiles)
          {
            var isEnabled = item.descriptor == null
                            || !item.descriptor.ModuleDescriptor.RequiresCreditCard
                            || profile.Profile.CreditCard != null;
            profile.IsEnabled = isEnabled;
          }
        });

      profilesStream
        .Sort(profilesSortComparer)
        .Bind(out _profiles)
        .DisposeMany()
        .Subscribe();


      var proxiesSortComparer = SortExpressionComparer<TaskProxyViewModel>.Descending(_ => _.IsEnabled)
        .ThenByAscending(_ => _.ProxyGroup.Name);
      var proxyGroupsStream = proxyGroupsRepository.Items.Connect()
        .Transform(pg => new TaskProxyViewModel(pg))
        .AutoRefresh(_ => _.IsEnabled)
        .Sort(proxiesSortComparer)
        .Bind(out _proxyGroups)
        .DisposeMany()
        .Publish()
        .RefCount();

      this.WhenAnyValue(_ => _.Stats)
        .CombineLatest(proxyGroupsStream.ToCollection(), (stats, proxyGroups) => (stats, proxyGroups))
        .Throttle(TimeSpan.FromMilliseconds(500))
        .ObserveOn(RxApp.MainThreadScheduler)
        .Subscribe(item =>
        {
          foreach (var model in item.proxyGroups)
          {
            model.IsEnabled =
              model.ProxyGroup.Proxies.Any(p => !item.stats?.Select(_ => _.ProxyId).Contains(p.Id) ?? true);
          }
        });

      proxyGroupsStream.Subscribe();

      SetupModuleDescriptorChangesHandler();
      this.WhenAnyValue(_ => _.SelectedModule)
        .Where(s => s != null)
        .Subscribe(s =>
        {
          if (_initialFieldValues == null)
          {
            return;
          }

          s.SetFields(_initialFieldValues);
        });


      CloseCommand = ReactiveCommand.Create(() => { });

      var canBeCreated = this.WhenAnyValue(_ => _.SelectedModuleDescriptor)
        .CombineLatest(
          this.WhenAnyValue(_ => _.SelectedProxyGroup),
          this.WhenAnyValue(_ => _.SelectedProfile),
          this.WhenAnyValue(_ => _.Quantity),
          (m, g, p, q) =>
            m != null && (m.ModuleDescriptor.NoProfileRequires || (g == null || g.ProxyGroup.HasAnyProxy) && p != null)
                      && q > 0
        );
      CreateCommand = ReactiveCommand.CreateFromTask(CreateTasksAsync, canBeCreated);

      FetchAndBuildFieldsCommand = ReactiveCommand.CreateFromTask(async ct =>
      {
        var url = new Uri(FormUrl);
        var result = await ((IDynamicFormsModule) SelectedModule).FetchFieldsAsync(url, ct);
        foreach (var warning in result.Warnings)
        {
          _toasts.Show(ToastContent.Warning(warning));
        }
      });

      FetchAndBuildFieldsCommand.IsExecuting
        .CombineLatest(CreateCommand.IsExecuting, (fetching, creating) => fetching || creating)
        .ObserveOn(RxApp.MainThreadScheduler)
        .ToPropertyEx(this, _ => _.IsBusy);

      CreateCommand.IsExecuting
        .ObserveOn(RxApp.MainThreadScheduler)
        .ToPropertyEx(this, _ => _.IsCreating);

      this.WhenAnyValue(_ => _.FormUrl)
        .Where(url => SelectedModule is IDynamicFormsModule gfm && gfm.IsUrlValid(url))
        .Select(_ => Unit.Default)
        .InvokeCommand(FetchAndBuildFieldsCommand);
    }

    private void SetupAvailableModulesChangeHandler()
    {
      var searchTerm = this.WhenAnyValue(_ => _.SearchTerm);

      this.WhenAnyValue(_ => _.Captchas)
        .CombineLatest(searchTerm, (c, search) => (captchas: c, search))
        .Select(filters =>
        {
          var modules = _modulesProvider.SupportedModules.Select(m =>
            new ModuleItemViewModel(m, IsModuleActive(m, filters.captchas)));

          if (!string.IsNullOrWhiteSpace(filters.search))
          {
            var normalizedSearch = filters.search.ToLowerInvariant();
            modules = modules.Where(m => m.ModuleDescriptor.Name.ToLowerInvariant().Contains(normalizedSearch));
          }

          return modules;
        })
        .Subscribe(m => Modules = m);
    }

    private static bool IsModuleActive(RaffleModuleDescriptor m, IEnumerable<CaptchaResolverDescriptor> captchas)
    {
      return !m.RequiresCaptcha || captchas.Any(c => c.IsActive);
    }

    private void SetupModuleDescriptorChangesHandler()
    {
      var moduleDescChanged = this.WhenAnyValue(_ => _.SelectedModuleDescriptor);
      moduleDescChanged.Select(_ => _?.ModuleDescriptor.NoProfileRequires == false)
        .ToPropertyEx(this, _ => _.RequiresProfile);

      moduleDescChanged
        .Subscribe(m =>
        {
          if (!m?.IsActive ?? false)
          {
            SelectedModuleDescriptor = null;
          }
        });

      CompositeDisposable moduleChanges = null;
      moduleDescChanged
        .Select(d => d == null ? null : _raffleModuleFactory.Create(d.ModuleDescriptor.ModuleType))
        .Do(m =>
        {
          moduleChanges?.Dispose();
          SelectedModule?.Dispose();
          SelectedModule = null;
          SelectedProfile = null;
          SelectedProxyGroup = null;
          AllRequiredFieldsAreFilled = false;
          FormUrl = null;
        })
        .Where(m => m != null)
        .Subscribe(newModule =>
        {
          moduleChanges = new CompositeDisposable();
          AllRequiredFieldsAreFilled =
            !newModule.AdditionalFields.All(_ => _.IsRequired) || !newModule.AdditionalFields.Any();

          var fieldChanges = new CompositeDisposable();
          Observable.FromEventPattern(h => newModule.AdditionalFieldsChanged += h,
              h => newModule.AdditionalFieldsChanged -= h)
            .Subscribe(_ =>
            {
              fieldChanges?.Dispose();
              fieldChanges = new CompositeDisposable();
              SubscribeForFieldChanges(newModule, moduleChanges);
            })
            .DisposeWith(moduleChanges);

          SubscribeForFieldChanges(newModule, moduleChanges);

          this.WhenAnyValue(_ => _.AllRequiredFieldsAreFilled)
            .Where(allRequiredFieldsAreFilled => allRequiredFieldsAreFilled)
            .CombineLatest(newModule.ComputedIdentityChanged, (_, id) => id)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .DistinctUntilChanged()
            .Select(id => _usageStatsService.GetStatsAsync(id).ToObservable())
            .Switch()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(stats => { Stats = stats; })
            .DisposeWith(moduleChanges);

          SelectedModule = newModule;
          IsDynamicFormsModule = SelectedModule is IDynamicFormsModule;
        });
    }

    public bool IsBusy { [ObservableAsProperty] get; }
    public bool IsCreating { [ObservableAsProperty] get; }
    public bool RequiresProfile { [ObservableAsProperty] get; }
    [Reactive] public int CreatedCount { get; private set; }
    [Reactive] public string SearchTerm { get; set; }

    private void SubscribeForFieldChanges(IRaffleModule newModule, CompositeDisposable fieldsChanges)
    {
      var fieldsOnChanges = newModule.AdditionalFields.Where(f => f.IsRequired)
        .Select(_ => _.Changed);
      fieldsOnChanges.CombineLatest()
        .Select(values => values.Count == 0 || values.All(f => !f.IsEmpty))
        .Subscribe(areFilled => AllRequiredFieldsAreFilled = areFilled)
        .DisposeWith(fieldsChanges);
    }

    private Func<TaskProfileViewModel, bool> BuildProfileFilter(ModuleItemViewModel moduleDescriptor)
    {
      return _ => moduleDescriptor != null;
    }

    private async Task CreateTasksAsync(CancellationToken ct)
    {
      CreatedCount = 0;
      await Task.Run(async () =>
      {
        var seed = Stopwatch.GetTimestamp();
        var fields = SelectedModule.AdditionalFields.ToArray();
        if (fields.Where(_ => _.IsRequired).Any(f => f.IsEmpty))
        {
          _toasts.Show(ToastContent.Error("Not all required fields are filled. Please check form and try again."));
          return;
        }

        var validationResults = await Task.WhenAll(fields.Select(async f =>
        {
          var isValid = await f.IsValid();
          return new
          {
            Field = f,
            IsValid = isValid
          };
        }));

        var invalidFields = validationResults.Where(_ => !_.IsValid).Select(_ => _.Field.DisplayName).ToArray();
        if (invalidFields.Any())
        {
          var invalidFieldNames = string.Join(", ", invalidFields);
          _toasts.Show(ToastContent.Error($"'{invalidFieldNames}' are invalid. Please check form and try again."));
          return;
        }

        var proxies = SelectedProxyGroup?.ProxyGroup.Proxies.Where(p => Stats.All(s => p.Id != s.ProxyId)).ToArray();
        // var tasks = new List<RaffleTask>(Quantity);
        var collisionsGuard = new SemaphoreSlim(1, 1);
        var hotTasks = Enumerable.Range(0, Math.Min(proxies?.Length ?? int.MaxValue, Quantity))
          .AsParallel()
          .Select(async idx =>
          {
            var module = _raffleModuleFactory.Create(SelectedModule);

            var task = new RaffleTask(seed + idx, module, proxies?[idx], SelectedProxyGroup?.ProxyGroup.Name,
              SelectedProfile?.Profile,
              Locator.Current.GetService<IHttpClientBuilder>(), Locator.Current);
            try
            {
              await task.InitializeAsync(ct);
              await collisionsGuard.WaitAsync(ct);
              CreatedCount++;
              collisionsGuard.Release();

              return task;
            }
            catch (OperationCanceledException)
            {
              return null;
            }
          });

        var tasks = (await Task.WhenAll(hotTasks))
          .Where(t => t != null)
          .ToList();

        var count = Math.Max(proxies?.Length ?? 0, tasks.Count);
        if (count < Quantity)
        {
          _toasts.Show(ToastContent.Warning(
            $"Created only '{count}' task(s) instead of '{Quantity}' because of uniqueness limitation or not enough proxies in the pool."));
        }

        _tasksService.AddRange(tasks);
        await Task.WhenAll(tasks.Select(t => Task.Run(() => t.PrepareAsync(ct), ct)));
        // _tasksService.AddRange(tasks);
        _toasts.Show(ToastContent.Success("Tasks created and initialized"));
      }, ct);
    }

    [Reactive] public bool IsDynamicFormsModule { get; private set; }
    public ReactiveCommand<Unit, Unit> FetchAndBuildFieldsCommand { get; private set; }
    [Reactive] public string FormUrl { get; set; }
    [Reactive] public IList<ModuleUsageStat> Stats { get; private set; }
    [Reactive] public bool AllRequiredFieldsAreFilled { get; private set; }
    [Reactive] public ModuleItemViewModel SelectedModuleDescriptor { get; set; }
    [Reactive] public TaskProxyViewModel SelectedProxyGroup { get; set; }
    [Reactive] public TaskProfileViewModel SelectedProfile { get; set; }
    [Reactive] public bool CanModuleBeChanged { get; private set; } = true;

    [Reactive] public IList<CaptchaResolverDescriptor> Captchas { get; private set; }
    [Reactive] public IRaffleModule SelectedModule { get; private set; }

    [Reactive] public IEnumerable<ModuleItemViewModel> Modules { get; private set; }
    public ReadOnlyObservableCollection<TaskProfileViewModel> Profiles => _profiles;
    public ReadOnlyObservableCollection<TaskProxyViewModel> ProxyGroups => _proxyGroups;

    [Reactive] public int Quantity { get; set; }

    public ReactiveCommand<Unit, Unit> CloseCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> CreateCommand { get; private set; }

    public void Reset()
    {
      _initialFieldValues = null;
      SelectedModuleDescriptor = null;
      SelectedProxyGroup = null;
      SelectedProfile = null;
      Quantity = 1;
      IsDynamicFormsModule = false;
      CanModuleBeChanged = true;
      Captchas = _captchaResolversProvider.SupportedCaptchaResolvers.ToList();
    }

    public void SelectModuleByName(string providerName, IDictionary<string, object> initialFieldValues)
    {
      CanModuleBeChanged = false;
      var descriptor = _modulesProvider.SupportedModules.FirstOrDefault(_ => _.Name == providerName);
      SelectedModuleDescriptor = new ModuleItemViewModel(descriptor, IsModuleActive(descriptor, Captchas));
      _initialFieldValues = initialFieldValues;
    }
  }
}