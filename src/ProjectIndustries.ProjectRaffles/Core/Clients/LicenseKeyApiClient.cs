using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Clients
{
  public class LicenseKeyApiClient : ApiClientBase, ILicenseKeyApiClient
  {
    private const string ProductId = "5f76e109d455e72971e22c3d";

    public LicenseKeyApiClient(ProjectIndustriesApiConfig apiConfig) : base(apiConfig)
    {
    }

    public async Task<AuthenticationResult> Authenticate(string licenseKey, string hwid, CancellationToken ct = default)
    {
      string path = $"/licenseKeys/{ProductId}/authenticate?sessionId={WebUtility.UrlEncode(hwid)}";
      try
      {
        var r = await GetAsync(path, licenseKey, ct);
        return JsonConvert.DeserializeObject<AuthenticationResult>(await r.Content.ReadAsStringAsync(ct));
      }
      catch (Exception)
      {
        return AuthenticationResult.CreateUnkownError();
      }
    }

    public async Task DeactivateOnCurrentDeviceAsync(string licenseKey, CancellationToken ct = default)
    {
      string path = $"/licenseKeys/reset";
      await GetAsync(path, licenseKey, ct);
    }
  }
}