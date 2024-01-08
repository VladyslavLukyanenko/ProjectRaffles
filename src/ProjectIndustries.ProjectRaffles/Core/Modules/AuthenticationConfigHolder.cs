using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public class AuthenticationConfigHolder
  {
    public static readonly Action<IModuleHttpClient, Account, string> DefaultJwtAuthHeaderConfigurer =
      (client, account, token) => { client.DefaultHeaders.Authorization = AuthenticationHeaderValue.Parse(token); };

    public static readonly Action<IModuleHttpClient, Account, string> NoopCredentialsConfigurer =
      (client, account, token) => { };

    private readonly Func<Account, CancellationToken, Task<string>> _authenticationHandler;
    private readonly Action<IModuleHttpClient, Account, string> _credentialsConfigurer;

    public AuthenticationConfigHolder(Func<Account, CancellationToken, Task<string>> authenticationHandler,
      Action<IModuleHttpClient, Account, string> credentialsConfigurer)
    {
      _authenticationHandler = authenticationHandler;
      _credentialsConfigurer = credentialsConfigurer;
    }

    public async Task AuthenticateAsync(Account account, IModuleHttpClient client, CancellationToken ct)
    {
      AuthenticationToken = TokenValidator != null && !await TokenValidator.Invoke(account, ct)
        ? await _authenticationHandler(account, ct)
        : account.AccessToken;

      _credentialsConfigurer(client, account, AuthenticationToken);
    }

    public Func<Account, CancellationToken, Task<bool>> TokenValidator { get; set; }

    public string AuthenticationToken { get; private set; }
  }
}