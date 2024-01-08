using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SlamjamModule
{
  public class SlamjamClient : ModuleHttpClientBase, ISlamjamClient
  {
    protected override void ConfigureHttpClient(HttpClientOptions options)
    {
      options.PostConfigure = httpClient =>
      {
        httpClient.DefaultRequestHeaders.Add("User-Agent",
          "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
        httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        httpClient.DefaultRequestHeaders.Add("dnt", "1");
      };
    }

    public async Task<bool> SubmitAsync(SlamjamSubmitPayload payload, CancellationToken ct)
    {
      //todo: get Datadome cookies, otherwise submitting will fail
      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"variantID", payload.Variant},
        {"firstName", payload.Profile.ShippingAddress.FirstName},
        {"lastName", payload.Profile.ShippingAddress.LastName},
        {"city", payload.Profile.ShippingAddress.City},
        {"address1", payload.Profile.ShippingAddress.AddressLine1},
        {
          "countryCode", payload.Profile.ShippingAddress.CountryId
        }, //todo: figure out their custom ID's, may need to scrape
        {"state", payload.Profile.ShippingAddress.ProvinceCode},
        {"zipCode", payload.Profile.ShippingAddress.ZipCode},
        {"phone", payload.Profile.ShippingAddress.PhoneNumber},
        {
          "prefix", payload.Profile.ShippingAddress.PhoneNumber
        } //todo: add phone prefixes to a file, already made as a switch statement
      });
      
      HttpClient.DefaultRequestHeaders.Add("referer", payload.RaffleUrl);
      
      var endpoint =
        $"https://www.slamjam.com/on/demandware.store/Sites-slamjam-Site/{payload.Store}/Raffle-AddRaffleProduct";
      var signup = await HttpClient.PostAsync(endpoint, content, ct);
      return signup.IsSuccessStatusCode;
    }
  }
}