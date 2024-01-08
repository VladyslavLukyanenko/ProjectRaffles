using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Accounts;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Accounts
{
  public class AccountsViewModel
    : PageViewModelBase, IRoutableViewModel
  {
    private readonly ReadOnlyObservableCollection<AccountGroup> _accountGroups;

    public AccountsViewModel(IAccountGroupsRepository accountGroupsRepository, IScreen hostScreen, IMessageBus messageBus,
      HeaderGenerateAccountsViewModel generateAccounts, IToastNotificationManager toasts)
      : base("Accounts", messageBus)
    {
      HostScreen = hostScreen;
      GenerateAccounts = generateAccounts;
      var items = accountGroupsRepository.Items.Connect();
      items
        .Bind(out _accountGroups)
        .DisposeMany()
        .Subscribe();

      items.ToCollection()
        .Where(i => i.Any())
        .Subscribe(i =>
        {
          if (SelectedAccountGroup == null)
          {
            SelectedAccountGroup = i.First();
          }
        });

      var canCreate = this.WhenAnyValue(_ => _.NewGroupName)
        .Select(grp => !string.IsNullOrWhiteSpace(grp));

      CreateCommand =
        ReactiveCommand.CreateFromTask(async ct => { await CreateAccountGroupAsync(accountGroupsRepository, ct); },
          canCreate);

      var canRemove = this.WhenAnyValue(_ => _.SelectedAccountGroup)
        .Select(g => g != null);
      RemoveGroupCommand = ReactiveCommand.CreateFromTask(async ct =>
      {
        await accountGroupsRepository.RemoveAsync(SelectedAccountGroup, ct);
        toasts.Show(ToastContent.Success("Group removed successfully"));
      }, canRemove);

      RemoveAccountCommand = ReactiveCommand.CreateFromTask<AccountRowViewModel>(async (acc, ct) =>
      {
        SelectedAccountGroup.Remove(acc.Account);
        AccountRows.Remove(acc);
        await accountGroupsRepository.SaveAsync(SelectedAccountGroup, ct);
        toasts.Show(ToastContent.Success("Account removed successfully"));
      });

      IDisposable accountGroupChanged = null;
      this.WhenAnyValue(_ => _.SelectedAccountGroup)
        .Subscribe(g =>
        {
          AccountRows.Clear();
          accountGroupChanged?.Dispose();
          if (g == null)
          {
            return;
          }

          AddAllRows();
          
          accountGroupChanged = g.Accounts.ObserveCollectionChanges()
            .Subscribe(_ =>
            {
              AccountRows.Clear();
              AddAllRows();
            });


          void AddAllRows()
          {
            using (AccountRows.SuspendNotifications())
            {
              AccountRows.AddRange(g.Accounts.Select(a => new AccountRowViewModel(a)));
            }
          }
        });
    }

    private async Task CreateAccountGroupAsync(IAccountGroupsRepository accountGroupsRepository, CancellationToken ct)
    {
      IEnumerable<Account> accounts = Enumerable.Empty<Account>();
      if (!string.IsNullOrWhiteSpace(RawAccounts))
      {
        accounts = RawAccounts.Split(new[] {"\n", Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
          .Select(token =>
          {
            var emailEndIdx = token.IndexOf(':');
            if (emailEndIdx == -1)
            {
              return null;
            }

            var email = token[..emailEndIdx];
            var pwd = token[(emailEndIdx + 1)..];

            return new Account(email, pwd);
          })
          .Where(a => a != null);
      }

      var group = new AccountGroup(NewGroupName, accounts);
      await accountGroupsRepository.SaveAsync(@group, ct);
      NewGroupName = null;
      RawAccounts = null;
    }


    public ObservableCollectionExtended<AccountRowViewModel> AccountRows { get; } =
      new ObservableCollectionExtended<AccountRowViewModel>();

    public ReadOnlyObservableCollection<AccountGroup> AccountGroups => _accountGroups;
    [Reactive] public AccountGroup SelectedAccountGroup { get; set; }
    [Reactive] public string NewGroupName { get; set; }
    [Reactive] public string RawAccounts { get; set; }
    public string UrlPathSegment => nameof(AccountsViewModel);
    public IScreen HostScreen { get; }
    public HeaderGenerateAccountsViewModel GenerateAccounts { get; }


    public ReactiveCommand<Unit, Unit> CreateCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> RemoveGroupCommand { get; private set; }
    public ReactiveCommand<AccountRowViewModel, Unit> RemoveAccountCommand { get; private set; }

    protected override ViewModelBase GetHeaderContent() => GenerateAccounts;
  }
}