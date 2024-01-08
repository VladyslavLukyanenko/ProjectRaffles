using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Clients
{
  public interface IUpdateApiClient
  {
    Task<Version> GetLatestAvailableVersionAsync(string licenseKey, CancellationToken ct = default);

    Task DownloadInstallerAsync(Stream output, Version version, bool isX64, ProgressChanged onProgressCallback,
      CancellationToken ct = default);
  }
}