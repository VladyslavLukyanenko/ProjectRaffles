using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.OffTheHookModule
{
  public class OffTheHookClient : ModuleHttpClientBase, IOffTheHookClient
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
      var body = await getPage.ReadStringResultOrFailAsync("Can't access site", ct);

      var doc = new HtmlDocument();
      doc.LoadHtml(body);
      
      var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;
      var title = node.Replace(" | Off The Hook Montreal", "").Replace("\n", "").Replace("  ", "");

      return title;
    }

    public async Task<OffTheHookParsedRaffle> ParseSiteAsync(string raffleUrl, CancellationToken ct)
    {
      var klaviyoUrl = "https://fast.a.klaviyo.com/forms/api/v3/full-forms?company_id=KTMsNh";

      var getRaffle = await HttpClient.GetAsync(raffleUrl, ct);
      var raffleHtml = await getRaffle.ReadStringResultOrFailAsync("Can't access raffle", ct);

      //get raffle ID
      var regexPattern = @"<div class=""klaviyo-form-.*""><\/div><\/div>";
      var raffleRegex = new Regex(regexPattern);
      var raffleRegexed = raffleRegex.Match(raffleHtml).ToString();
      var raffleId = raffleRegexed.Replace(@"<div class=""klaviyo-form-", "").Replace(@"""></div></div>", "");

      //get source
      var doc = new HtmlDocument();
      doc.LoadHtml(raffleHtml);

      var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;
      var source = node.Replace(" | Off The Hook Montreal", "").Replace("\n", "").Replace("  ", "");

      //get the form version ID
      var getScript = await HttpClient.GetAsync(klaviyoUrl, ct);
      var scriptHtml = await getScript.ReadStringResultOrFailAsync("Can't access form", ct);

      var forms = JsonConvert.DeserializeObject<List<OffTheHookForms.Form>>(scriptHtml);
      var raffleForm = forms.First(form => form.FormId == raffleId);

      var formVersion = raffleForm.LiveFormVersions.First().FormVersionId.ToString();

      return new OffTheHookParsedRaffle(raffleId, source, formVersion);
    }


    public async Task<bool> SubmitAsync(AddressFields profile, string email, OffTheHookParsedRaffle parsedRaffle, string size,
      string instagram, CancellationToken ct)
    {
      var endpoint = "https://a.klaviyo.com/ajax/subscriptions/subscribe";

      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"g", "Rm2STB"},
        {
          "$fields",
          "$source,$first_name,$last_name,Shoe Size,Instagram,$email,$consent_method,$consent_form_id,$consent_form_version,services"
        },
        {"$list_fields", ""},
        {"$timezone_offset", "2"},
        {"$source", parsedRaffle.Source},
        {"$first_name", profile.FirstName.Value},
        {"$last_name", profile.LastName.Value},
        {"Shoe Size", size},
        {"Instagram", instagram},
        {"$email", email},
        {"$consent_method", "Klaviyo Form"},
        {"$consent_form_id", parsedRaffle.RaffleId},
        {"$consent_form_version", parsedRaffle.FormVersion},
        {"services", @"{""shopify"":{""source"":""form""}}"}
      });

      var post = await HttpClient.PostAsync(endpoint, content, ct);
      var postContent = await post.ReadStringResultOrFailAsync("Error on submission", ct);
      
      if(!postContent.Contains(@"""success:"": true")) await post.FailWithRootCauseAsync("Submission error", ct);

      return postContent.Contains(@"""success"": true");
    }
  }
}