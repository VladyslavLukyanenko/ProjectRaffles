using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public interface IDynamicFormsModule
  {
    ValueTask<FormParseResult> FetchFieldsAsync(Uri formUrl, CancellationToken ct = default);
    bool IsUrlValid(string formUrl);
  }
}