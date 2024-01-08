using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Clients
{
  public class RafflesApiClient : ApiClientBase, IRafflesApiClient
  {
    private readonly ILogger<RafflesApiClient> _logger;
    public RafflesApiClient(ProjectIndustriesApiConfig apiConfig, ILogger<RafflesApiClient> logger)
      : base(apiConfig)
    {
      _logger = logger;
    }

    public async Task<IList<SubmissionStatsEntry>> GetStatsAsync(string licenseKey,
      CancellationToken ct = default)
    {
      try
      {
        var message = new HttpRequestMessage(HttpMethod.Get, ApiConfig.ResolveUrl("/raffles/api/stats"))
        {
          Headers = {{"Auth", licenseKey}}
        };

        var response = await HttpClient.SendAsync(message, HttpCompletionOption.ResponseContentRead, ct);
        string json = await response.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<List<SubmissionStatsEntry>>(json);
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, exc.Message);
        return Array.Empty<SubmissionStatsEntry>();
      }
    }
  }
}