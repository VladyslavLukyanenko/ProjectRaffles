using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Accounts;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public abstract class AccountBasedRaffleModuleBase<T> : RegularRaffleModuleBase<T>
    where T : IModuleHttpClient
  {
    // ReSharper disable once StaticMemberInGenericType
    private static readonly ConcurrentDictionary<Account, SemaphoreSlim> AuthGates =
      new ConcurrentDictionary<Account, SemaphoreSlim>();

    private readonly AccountPickerField _accountField = new AccountPickerField("Participation Accounts");
    protected Account SelectedAccount;
    protected AuthenticationConfigHolder AuthenticationConfig;
    protected readonly bool PersistToken;

    protected AccountBasedRaffleModuleBase(T client, string raffleUrlRegexPattern, bool persistToken = false,
      bool skipFieldsInitialization = false)
      : base(client, raffleUrlRegexPattern, true)
    {
      PersistToken = persistToken;
      if (!skipFieldsInitialization)
      {
        _ = InitializeDefaultIdentityFields();
      }
    }

    protected virtual RaffleStatus LoggingIntoAccountStatus => RaffleStatus.LoggingIntoAccount;

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return _accountField;
    }

    protected override async Task ExecuteAsync(Profile profile, CancellationToken ct)
    {
      try
      {
        SelectedAccount = _accountField.GetNextAccount();
        SemaphoreSlim authGates = AuthGates.GetOrAdd(SelectedAccount, _ => new SemaphoreSlim(1, 1));
        await authGates.WaitAsync(ct);
        if (AuthenticationConfig != null)
        {
          Status = LoggingIntoAccountStatus;
          await AuthenticationConfig.AuthenticateAsync(SelectedAccount, HttpClient, ct);
          if (PersistToken)
          {
            var svc = ExecutionContext.DependencyResolver.GetService<IAccountGroupsRepository>();
            SelectedAccount.AccessToken = AuthenticationConfig.AuthenticationToken;
            await svc.SaveSilentlyAsync(_accountField.Value, ct);
          }
        }

        authGates.Release();
        await base.ExecuteAsync(profile, ct);
      }
      finally
      {
        AuthGates.TryRemove(SelectedAccount, out _);
      }
    }
  }
}