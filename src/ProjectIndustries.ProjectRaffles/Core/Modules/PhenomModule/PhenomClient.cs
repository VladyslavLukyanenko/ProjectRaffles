using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.PhenomModule
{
  public class PhenomClient : ModuleHttpClientBase, IPhenomClient
  {
    private readonly ICountriesService _countriesService;

    public PhenomClient(ICountriesService countriesService)
    {
      _countriesService = countriesService;
    }

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

    public async Task<PhenomParsedRaffle> ParseRaffleAsync(string raffleurl, CancellationToken ct)
    {
      var getPage = await HttpClient.GetAsync(raffleurl, ct);
      var body = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

      var doc = new HtmlDocument();
      doc.LoadHtml(body);

      var formId = doc.DocumentNode.SelectSingleNode("//input[@name='form_id']").GetAttributeValue("value", "");

      var formBuildId = doc.DocumentNode.SelectSingleNode("//input[@name='form_build_id']")
        .GetAttributeValue("value", "");

      return new PhenomParsedRaffle(formBuildId, formId);
    }


    public async Task<bool> SubmitAsync(PhenomSubmitPayload payload, CancellationToken ct)
    {
      var country = _countriesService.GetCountryName(payload.Profile.CountryId.Value)
        .Replace(" (" + payload.Profile.CountryId.Value + ")", "");
      ;
      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"submitted[name]", payload.Profile.FullName.Value},
        {"submitted[email]", payload.Email},
        {"submitted[address][country]", country},
        {"submitted[address][thoroughfare]", payload.Profile.AddressLine1.Value},
        {"submitted[address][premise]", ""},
        {"submitted[address][locality]", payload.Profile.City.Value},
        {"submitted[address][administrative_area]", payload.Profile.ProvinceId.Value},
        {"submitted[address][postal_code]", payload.Profile.PostCode.Value},
        {"submitted[shoe_size]", payload.SizeValue},
        {"submitted[ig_handle]", payload.InstagramHandle},
        {"submitted[terms][1]", "1"},
        {"details[sid]", ""},
        {"details[page_num]", "1"},
        {"details[page_count]", "1"},
        {"details[finished]", "0"},
        {"form_build_id", payload.ParsedRaffle.FormBuildId},
        {"form_id", payload.ParsedRaffle.FormId},
        {"op", "Submit"}
      });

      var endpoint = "https://apps.shopmonkey.nl/panthers/raffle/raffle.php";
      var signup = await HttpClient.PostAsync(endpoint, content, ct);
      
      if(!signup.IsSuccessStatusCode) await signup.FailWithRootCauseAsync("Error on entry", ct);
      
      return signup.IsSuccessStatusCode;
    }
  }
}