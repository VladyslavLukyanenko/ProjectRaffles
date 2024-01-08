using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers
{
  public interface IFormParser
  {
    bool IsModuleSupported(RaffleModuleType moduleType);
    ValueTask<FormParseResult> ParseAsync(Uri url, CancellationToken ct = default);
  }
}