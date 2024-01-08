using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public class UserDataLocatedSettingsService : ISettingsService
  {
    private readonly ILogger<UserDataLocatedSettingsService> _logger;
    private const string StorageFolderName = "ProjectRaffles";

    private static readonly string StorageLocation =
      Path.Combine(GetLocalAppDataFolder(), StorageFolderName);

    private static readonly SemaphoreSlim ReadSemaphore = new SemaphoreSlim(1, 1);
    private static readonly SemaphoreSlim WriteSemaphore = new SemaphoreSlim(1, 1);

    public UserDataLocatedSettingsService(ILogger<UserDataLocatedSettingsService> logger)
    {
      _logger = logger;
    }

    private static string GetLocalAppDataFolder()
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
        return Environment.GetEnvironmentVariable("LOCALAPPDATA");
      }
      //
      // if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
      // {
      //   return Environment.GetEnvironmentVariable("XDG_DATA_HOME") ??
      //          Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".local", "share");
      // }
      //
      // if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
      // {
      //   return Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Library", "Application Support");
      // }

      throw new NotImplementedException("Unknown OS Platform");
    }

    public async Task<T> ReadSettingsOrDefaultAsync<T>(string name, Func<T> defaultFactory = null,
      CancellationToken ct = default)
    {
      try
      {
        await ReadSemaphore.WaitAsync(ct);
        var fullPath = GetSettingsFullPathOrDefault(name);
        _logger.LogDebug("Reading settings from path '{FullPath}' by key '{Key}'", fullPath, name);
        if (!File.Exists(fullPath))
        {
          _logger.LogDebug("File not found. Returning default value");
          return defaultFactory != null ? defaultFactory.Invoke() : default;
        }

        _logger.LogDebug("File found. Reading...");
        var content = await File.ReadAllTextAsync(fullPath, ct);

        _logger.LogDebug("Deserializing");
        return JsonConvert.DeserializeObject<T>(content);
      }
      finally
      {
        _logger.LogDebug("Read finished");
        ReadSemaphore.Release();
      }
    }

    public async Task WriteSettingsAsync<T>(string name, T settings, CancellationToken ct = default)
    {
      try
      {
        await WriteSemaphore.WaitAsync(ct);
        var fullPath = GetSettingsFullPathOrDefault(name);
        var json = JsonConvert.SerializeObject(settings);
        await File.WriteAllTextAsync(fullPath, json, ct);
      }
      finally
      {
        WriteSemaphore.Release();
      }
    }

    private string GetSettingsFullPathOrDefault(string name)
    {
      if (!Directory.Exists(StorageLocation))
      {
        _logger.LogDebug("Store location doesn't exists. Creating '{StorageLocation}'", StorageLocation);
        Directory.CreateDirectory(StorageLocation);
        _logger.LogDebug("Store location created");
      }

      return Path.Combine(StorageLocation, name);
    }
  }
}