using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Accounts;
using ProjectIndustries.ProjectRaffles.Core.Services.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Profiles;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Accounts
{
  public class GenerateAccountsWindowViewModel : ViewModelBase
  {
    private readonly IAccountGroupsRepository _accountGroupsRepository;
    private readonly IToastNotificationManager _toasts;
    private readonly ReadOnlyObservableCollection<AccountGroup> _accountGroups;
    private readonly ReadOnlyObservableCollection<Profile> _profiles;

    public GenerateAccountsWindowViewModel(IRaffleModulesProvider modulesProvider,
      IAccountGroupsRepository accountGroupsRepository, IToastNotificationManager toasts,
      IProfilesRepository profilesRepository)
    {
      _accountGroupsRepository = accountGroupsRepository;
      _toasts = toasts;
      accountGroupsRepository.Items.Connect()
        .Bind(out _accountGroups)
        .DisposeMany()
        .Subscribe();

      profilesRepository.Items.Connect()
        .Bind(out _profiles)
        .DisposeMany()
        .Subscribe();

      Modules = modulesProvider.SupportedModules
        .Where(_ => _.AccountGeneratorType != null)
        .ToArray();

      this.WhenAnyValue(_ => _.SelectedModule)
        .Select(module => module == null ? null : Locator.Current.GetService(module.AccountGeneratorType))
        .Cast<IAccountGenerator>()
        .ToPropertyEx(this, _ => _.SelectedGenerator);

      this.WhenAnyValue(_ => _.NewAccountGroupName)
        .Where(n => !string.IsNullOrWhiteSpace(n))
        .Subscribe(_ => { SelectedAccountGroup = null; });

      this.WhenAnyValue(_ => _.SelectedAccountGroup)
        .Subscribe(_ => { GeneratedAccounts.Clear(); });

      this.WhenAnyValue(_ => _.SelectedAccountGroup)
        .Where(g => g != null)
        .Subscribe(_ => { NewAccountGroupName = null; });

      var falseResult = Observable.Return(false);
      var canGenerate = this.WhenAnyValue(_ => _.SelectedGenerator)
        .Select(g => g == null
          ? falseResult
          : g.ConfigurationFields
            .Where(_ => _.IsRequired)
            .Select(_ => _.Changed)
            .CombineLatest(_ => _)
            .Select(fields => fields.Select(f => f.IsValid().ToObservable())
              .CombineLatest(_ => _.All(isValid => isValid)))
            .Switch())
        .Switch()
        .CombineLatest(
          this.WhenAnyValue(_ => _.SelectedAccountGroup),
          this.WhenAnyValue(_ => _.NewAccountGroupName),
          this.WhenAnyValue(_ => _.SelectedModule),
          this.WhenAnyValue(_ => _.Quantity).Select(q => q > 0),
          (isGeneratorValid, accGrp, newGroupName, mod, isQtyValid) =>
            isGeneratorValid && (accGrp != null || !string.IsNullOrWhiteSpace(newGroupName)) && mod != null
            && isQtyValid);

      this.WhenAnyValue(_ => _.SelectedGenerator)
        .Where(g => g != null)
        .Subscribe(async generator =>
        {
          await generator.InitializeAsync(PickRandomProfile
            ? RoundRobinProfileStrategy()
            : SelectedProfileStrategy());
        });

      this.WhenAnyValue(_ => _.PickRandomProfile)
        .CombineLatest(this.WhenAnyValue(_ => _.SelectedProfile), (b, profile) => b)
        .Subscribe(_ => Reset());
      this.WhenAnyValue(_ => _.PickRandomProfile)
        .CombineLatest(this.WhenAnyValue(_ => _.SelectedProfile), (pick, profile) => pick || profile != null)
        .ToPropertyEx(this, _ => _.ProfileRetriveStrategySelected);

      GenerateCommand = ReactiveCommand.CreateFromTask(GenerateAccountAsync, canGenerate);

      RemoveAccountCommand =
        ReactiveCommand.CreateFromTask<GeneratedAccount>(async (acc, ct) => await RemoveAccountAsync(acc, ct));
    }

    private Func<Profile> SelectedProfileStrategy() => () => SelectedProfile;

    private Func<Profile> RoundRobinProfileStrategy()
    {
      var counter = 0;
      var profiles = _profiles.ToList();
      return () =>
      {
        var idx = counter++;
        if (idx > profiles.Count - 1)
        {
          counter = 0;
          idx = 0;
        }

        return profiles[idx];
      };
    }

    private async Task RemoveAccountAsync(GeneratedAccount acc, CancellationToken ct)
    {
      GeneratedAccounts.Remove(acc);
      SelectedAccountGroup.Remove(acc.Account);
      await _accountGroupsRepository.SaveSilentlyAsync(SelectedAccountGroup, ct);
    }

    private async Task GenerateAccountAsync(CancellationToken ct)
    {
      GeneratedAccounts.Clear();
      if (_profiles.Count == 0)
      {
        _toasts.Show(ToastContent.Error("No profiles found"));
        return;
      }

      await SelectedGenerator.PrepareAsync(ct);

      var group = SelectedAccountGroup;
      if (!string.IsNullOrWhiteSpace(NewAccountGroupName))
      {
        group = new AccountGroup(NewAccountGroupName);
        await _accountGroupsRepository.SaveSilentlyAsync(group, ct);
      }

      var initialCount = group.Accounts.Count;
      await foreach (var account in SelectedGenerator.GenerateAsync(ct))
      {
        var generatedAccount = new GeneratedAccount(SelectedModule, SelectedAccountGroup, account);
        GeneratedAccounts.Add(generatedAccount);
        group.Accounts.Add(generatedAccount.Account);

        if (Quantity == group.Accounts.Count - initialCount)
        {
          break;
        }

        await SelectedGenerator.PrepareAsync(ct);
      }

      await _accountGroupsRepository.SaveSilentlyAsync(group, ct);

      // _toasts.Show(ToastContent.Success("Accounts generated successfully")); // bug: it hides modal window once notification appears
    }

    public bool ProfileRetriveStrategySelected { [ObservableAsProperty] get; }

    [Reactive] public RaffleModuleDescriptor SelectedModule { get; set; }
    [Reactive] public int Quantity { get; set; }
    [Reactive] public AccountGroup SelectedAccountGroup { get; set; }
    [Reactive] public Profile SelectedProfile { get; set; }
    [Reactive] public bool PickRandomProfile { get; set; }
    public IAccountGenerator SelectedGenerator { [ObservableAsProperty] get; }

    public IEnumerable<RaffleModuleDescriptor> Modules { get; }
    public ReadOnlyObservableCollection<AccountGroup> AccountGroups => _accountGroups;
    public ReadOnlyObservableCollection<Profile> Profiles => _profiles;

    [Reactive] public string NewAccountGroupName { get; set; }
    public ReactiveCommand<Unit, Unit> GenerateCommand { get; }
    public ReactiveCommand<GeneratedAccount, Unit> RemoveAccountCommand { get; }


    [Reactive]
    public ObservableCollectionExtended<GeneratedAccount> GeneratedAccounts { get; set; } =
      new ObservableCollectionExtended<GeneratedAccount>();

    public void Reset()
    {
      GeneratedAccounts.Clear();
      Quantity = 0;
      NewAccountGroupName = null;
      SelectedModule = null;
      SelectedAccountGroup = null;
    }
  }
}