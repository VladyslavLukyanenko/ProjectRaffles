using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Diagnostics
{
  public class ProcdumpBasedMemoryDumpCreator : IMemoryDumpCreator
  {
    private static readonly string ProcdumpRelativePath = Path.Combine("tools", "procdump64.exe");
    private const string Switches = "-accepteula -ma ";
    private readonly ILogger<ProcdumpBasedMemoryDumpCreator> _logger;

    public ProcdumpBasedMemoryDumpCreator(ILogger<ProcdumpBasedMemoryDumpCreator> logger)
    {
      _logger = logger;
    }

    public async Task<bool> CreateAsync(string outputFileName, CancellationToken ct = default)
    {
      try
      {
        var processId = Process.GetCurrentProcess().Id;
        var arguments = $"{Switches} {processId.ToString()} \"{outputFileName}\"";
        var dumpStartInfo = new ProcessStartInfo(ProcdumpRelativePath, arguments)
        {
          RedirectStandardOutput = true,
          CreateNoWindow = true
        };

        var memoryDumpProcess = new Process
        {
          StartInfo = dumpStartInfo
        };

        memoryDumpProcess.Start();
        while (!memoryDumpProcess.StandardOutput.EndOfStream)
        {
          _logger.LogDebug(await memoryDumpProcess.StandardOutput.ReadToEndAsync());
          memoryDumpProcess.StandardOutput.DiscardBufferedData();
        }

        await memoryDumpProcess.WaitForExitAsync(ct);
        return true;
      }
      catch (Exception e)
      {
        _logger.LogError(e, "Can't create memory dump");
        return false;
      }
    }
  }
}