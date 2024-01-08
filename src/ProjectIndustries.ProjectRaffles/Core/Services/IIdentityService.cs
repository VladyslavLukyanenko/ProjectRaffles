using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Clients;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
    public interface IIdentityService
    {
        User CurrentUser { get; }
        IObservable<User> User { get; }
        IObservable<bool> IsAuthenticated { get; }
        Task<AuthenticationResult> TryAuthenticateAsync(CancellationToken ct = default);
        Task<AuthenticationResult> FetchIdentityAsync(CancellationToken ct = default);
        void Authenticate(AuthenticationResult result);
        void LogOut();
        Task DeactivateAsync(CancellationToken ct = default);
    }
}