using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public interface IGeneralSettingsService
  {
    Task InitializeAsync(CancellationToken ct = default);
    IObservable<GeneralSettings> Settings { get; }
    GeneralSettings CurrentSettings { get; }
    Task SaveAsync(GeneralSettings settings, CancellationToken ct = default);
  }
}