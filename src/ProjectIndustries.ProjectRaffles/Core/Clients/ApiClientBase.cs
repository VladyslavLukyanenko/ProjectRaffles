using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Clients
{
  public abstract class ApiClientBase
  {
    protected readonly HttpClient HttpClient = new HttpClient
    {
      DefaultRequestHeaders =
      {
        {
          "User-Agent",
          "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.132 Safari/537.36"
        },
        {
          "Accept", "*/*"
        }
      }
    };

    protected readonly ProjectIndustriesApiConfig ApiConfig;

    protected ApiClientBase(ProjectIndustriesApiConfig apiConfig)
    {
      ApiConfig = apiConfig;
    }

    protected async Task<HttpResponseMessage> PostAsync(string relativeUrl, string licenseKey, object payload,
      CancellationToken ct = default)
    {
      var message = new HttpRequestMessage(HttpMethod.Post, ApiConfig.ResolveUrl(relativeUrl))
      {
        Headers = {{"Auth", licenseKey}},
        Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
      };

      var response = await HttpClient.SendAsync(message, HttpCompletionOption.ResponseContentRead, ct);
      return response;
    }

    protected async Task<T> PostAsync<T>(string relativeUrl, string licenseKey, object payload,
      CancellationToken ct = default)
    {
      try
      {
        var response = await PostAsync(relativeUrl, licenseKey, payload, ct);
        if (!response.IsSuccessStatusCode)
        {
          return default;
        }

        return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync(ct));
      }
      catch
      {
        return default;
      }
    }

    protected async Task<HttpResponseMessage> GetAsync(string relativeUrl, string licenseKey,
      CancellationToken ct = default)
    {
      var message = new HttpRequestMessage(HttpMethod.Get, ApiConfig.ResolveUrl(relativeUrl))
      {
        Headers = {{"Auth", licenseKey}},
      };

      var response = await HttpClient.SendAsync(message, HttpCompletionOption.ResponseContentRead, ct);
      return response;
    }

    protected async Task<T> GetAsync<T>(string relativeUrl, string licenseKey, CancellationToken ct = default)
    {
      try
      {
        var r = await GetAsync(relativeUrl, licenseKey, ct);
        if (!r.IsSuccessStatusCode)
        {
          return default;
        }

        return JsonConvert.DeserializeObject<T>(await r.Content.ReadAsStringAsync(ct));
      }
      catch
      {
        return default;
      }
    }
  }
}