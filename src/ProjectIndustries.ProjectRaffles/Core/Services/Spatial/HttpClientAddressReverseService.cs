using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.WpfUI.MapBox.MapboxNetCore;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Spatial
{
  public class HttpClientAddressReverseService : IAddressReverseService
  {
    private const string AddressFormat = "https://nominatim.openstreetmap.org/reverse?lon={0}&lat={1}&format=json";
    private static DateTimeOffset? _lastRequest;
    private static readonly TimeSpan RequestsDelay = TimeSpan.FromSeconds(1);
    private static readonly SemaphoreSlim Gates = new SemaphoreSlim(1, 1);

    private readonly HttpClient _httpClient;

    public HttpClientAddressReverseService()
    {
      _httpClient = new HttpClient
      {
        DefaultRequestHeaders =
        {
          {
            "User-Agent",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36"
          }
        }
      };
    }

    public async Task<ReversedLocation> ReverseAddressAsync(GeoLocation point, CancellationToken ct = default)
    {
      try
      {
        await Gates.WaitAsync(CancellationToken.None);
        var diff = DateTimeOffset.UtcNow - _lastRequest.GetValueOrDefault(DateTimeOffset.UtcNow);
        _lastRequest = DateTimeOffset.UtcNow;
        if (diff < RequestsDelay)
        {
          await Task.Delay(RequestsDelay, ct);
        }

        var message =
          new HttpRequestMessage(HttpMethod.Get, string.Format(AddressFormat, point.Longitude, point.Latitude));
        var response = await _httpClient.SendAsync(message, ct);
        if (!response.IsSuccessStatusCode)
        {
          return null;
        }

        var content = await response.Content.ReadAsStringAsync(ct);
        var address = JsonConvert.DeserializeObject<ReversedLocation>(content);
        return address;
      }
      finally
      {
        Gates.Release();
      }
    }
  }
}