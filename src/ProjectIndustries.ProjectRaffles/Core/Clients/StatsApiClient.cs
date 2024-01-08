using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Clients
{
  public class StatsApiClient : ApiClientBase, IStatsApiClient
  {
    public StatsApiClient(ProjectIndustriesApiConfig apiConfig)
      : base(apiConfig)
    {
    }

    public async Task<bool> SubmitStatsAsync(SubmissionStatsEntry stats, string licenseKey,
      CancellationToken ct = default)
    {
      var message = new HttpRequestMessage(HttpMethod.Post, ApiConfig.ResolveUrl("/statistics"))
      {
        Headers = {{"Auth", licenseKey}},
        Content = new StringContent(JsonConvert.SerializeObject(new StatsData<SubmissionStatsEntry>(stats)),
          Encoding.UTF8, "application/json")
      };

      var response = await HttpClient.SendAsync(message, HttpCompletionOption.ResponseContentRead, ct);
      return response.IsSuccessStatusCode;
    }
  }
}