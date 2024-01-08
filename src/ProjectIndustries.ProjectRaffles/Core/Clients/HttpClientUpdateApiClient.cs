using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Clients
{
  public class HttpClientUpdateApiClient : ApiClientBase, IUpdateApiClient
  {
    private readonly ProjectIndustriesApiConfig _config;
    private static readonly Version NullVersion = new Version(0, 0, 0, 0);
    private const int BufferSize = 4096;

    public HttpClientUpdateApiClient(ProjectIndustriesApiConfig config): base(config)
    {
      _config = config;
    }

    public async Task<Version> GetLatestAvailableVersionAsync(string licenseKey, CancellationToken ct = default)
    {
      var r = await GetAsync("/updates/project-raffles/stable/win", licenseKey, ct);
      if (!r.IsSuccessStatusCode)
      {
        return NullVersion;
      }
      
      var rawVersion = await r.Content.ReadAsStringAsync(ct);
      if (!Version.TryParse(rawVersion, out var version))
      {
        return NullVersion;
      }

      return version;
    }

    public async Task DownloadInstallerAsync(Stream output, Version version, bool isX64, ProgressChanged onProgressCallback,
      CancellationToken ct = default)
    {
      using var response = await HttpClient.GetAsync(GetStableChannelInstallerUrl(version, isX64),
        HttpCompletionOption.ResponseHeadersRead, ct);
      using var contentStream = await response.Content.ReadAsStreamAsync();

      double installerSize = response.Content.Headers.ContentLength ?? 0L;
      long totalDownloaded = 0L;
      int readBytes;
      byte[] buffer = new byte[BufferSize];
      do
      {
        readBytes = await contentStream.ReadAsync(buffer, 0, BufferSize, ct);
        totalDownloaded += readBytes;
        await output.WriteAsync(buffer, 0, readBytes, ct);

        var progress = (int) Math.Floor(totalDownloaded / installerSize * 100);
        onProgressCallback((long) installerSize, totalDownloaded,progress);
      } while (readBytes > 0);
    }

    private Uri GetStableChannelInstallerUrl(Version version, bool isX64)
    {
      var arch = isX64 ? "x64" : "x86";
      return new Uri($"{_config.ApiHostName}/dist/project-raffles/stable/win/project-raffles-{arch}-{version}.exe");
    }
  }
}