using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SesinkoModule
{
  public class SesinkoClient : ModuleHttpClientBase, ISesinkoClient
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

    public async Task<SesinkoParsedRaffle> ParseRaffleAsync(string raffleurl, CancellationToken ct)
    {
      var getBody = await HttpClient.GetAsync(raffleurl, ct);
      var body = await getBody.ReadStringResultOrFailAsync("Can't access page", ct);
      
      var doc = new HtmlDocument();
      doc.LoadHtml(body);

      var formId = doc.DocumentNode.SelectSingleNode("//div[@class='pxFormGenerator']").GetAttributeValue("id", "");


      return new SesinkoParsedRaffle(formId);
    }

    public Task<string> CraftFormDataAsync(SesinkoSubmitPayload payload)
    {
      var rafflejson = new
      {
        FullName = payload.Profile.FullName.Value,
        EmailAddress = payload.Email,
        Instagram = payload.InstagramHandle,
        PickUpLocation = payload.PickupLocation,
        Size = payload.SizeValue,
        City = payload.Profile.City.Value + ", " + payload.Profile.ProvinceId.Value
      };

      var raffledata = JsonConvert.SerializeObject(rafflejson);
      var formdata = raffledata.Replace("FullName", "Full Name").Replace("Size", "Size (US)")
        .Replace("City", "City, State");

      return Task.FromResult(formdata);
    }

    public async Task<bool> SubmitAsync(string formdata, SesinkoSubmitPayload payload, CancellationToken ct)
    {
      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"form_uuid", payload.ParsedRaffle.FormId},
        {"formResponse", formdata},
        {"confirmationMail", payload.Email},
        {"is_pro", "false"}
      });
      HttpClient.DefaultRequestHeaders.Add("referer",
        $"https://formbuilder.hulkapps.com/corepage/customform?id={payload.ParsedRaffle.FormId}");

      var endpoint = "https://formbuilder.hulkapps.com/ajaxcall/formresponse";
      var signup = await HttpClient.PostAsync(endpoint, content, ct);
      if(!signup.IsSuccessStatusCode) await signup.FailWithRootCauseAsync("Error on entry", ct);
      
      return signup.IsSuccessStatusCode;
    }
  }
}