using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing
{
  public interface IMailChimpLandingPageFormParser
  {
    ValueTask<MailChimpParseResult> ParseAsync(Uri fieldsConfigUrl, HttpClient client, CancellationToken ct = default);
  }
}