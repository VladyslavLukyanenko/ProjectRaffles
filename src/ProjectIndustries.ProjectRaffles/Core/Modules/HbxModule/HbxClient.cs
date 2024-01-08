using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.HbxModule
{
  public class HbxClient : ModuleHttpClientBase, IHbxClient
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

    public async Task<string> GetProductAsync(string raffleurl, CancellationToken ct)
    {
      var getPage = await HttpClient.GetAsync(raffleurl, ct);
      var body = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

      var doc = new HtmlDocument();
      doc.LoadHtml(body);
      
      var model = doc.DocumentNode.SelectSingleNode("//div[@class='model']").InnerText;

      return model;
    }

    public async Task<HbxParsedRaffle> ParseProductAsync(string url, CancellationToken ct)
    {
      var getBody = await HttpClient.GetAsync(url, ct);
      var body = await getBody.ReadStringResultOrFailAsync("Can't access page", ct);

      var doc = new HtmlDocument();
      doc.LoadHtml(body);

      var model = doc.DocumentNode.SelectSingleNode("//div[@class='model']").InnerText;
      var raffleId = doc.DocumentNode.SelectSingleNode("//input[@name='raffles-campaign-monitor-list-id']")
        .GetAttributeValue("value", "");
      var productId = doc.DocumentNode.SelectSingleNode("//input[@name='product-id']").GetAttributeValue("value", "");

      return new HbxParsedRaffle(model, raffleId, productId);
    }

    public async Task<bool> SubmitAsync(AddressFields profile, string email, HbxParsedRaffle parsedRaffle, string captcha,
      string size, CancellationToken ct)
    {
      var address = profile.AddressLine1 + ", " + profile.City + ", " +
                    profile.ProvinceId;

      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"raffles-campaign-monitor-list-id", parsedRaffle.RaffleId},
        {"product-id", parsedRaffle.ProductId},
        {"raffles-first-name", profile.FirstName.Value},
        {"raffles-last-name", profile.LastName.Value},
        {"raffles-email", email},
        {"raffles-phone", profile.PhoneNumber.Value},
        {"raffles-address", address},
        {"raffles-country", profile.CountryId.Value},
        {"raffles-postal-code", profile.PostCode.Value},
        {"raffles-model", parsedRaffle.Model},
        {"raffles-size", $"US " + size},
        {"raffles-terms", "on"},
        {"g-recaptcha-response", captcha}
      });

      var endpointParams = $"name={parsedRaffle.Model}&email={email}";
      var endEscaped = endpointParams.UriEscape();

      var endpoint = "https://hbx.com/launch/success?event-type=raffle&" + endEscaped;

      var resp1 = await HttpClient.PostAsync(endpoint, content, ct);
      var respHtml = await resp1.ReadStringResultOrFailAsync("Error on submission", ct);
      
      if(!respHtml.Contains("Your entry has been successfully submitted")) await resp1.FailWithRootCauseAsync("Submission error", ct);

      return respHtml.Contains("Your entry has been successfully submitted");
    }
  }
}