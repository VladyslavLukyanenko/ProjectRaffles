using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SOTFModule
{
  public class SOTFClient : ModuleHttpClientBase, ISOTFClient
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

    public async Task<string> ParseRaffleAsync(CancellationToken ct)
    {
      //todo: cloudflare cookies needed for getting the page
      var raffleurl = "https://www.sotf.com/en/raffle_online_form.php";
      var getRaffle = await HttpClient.GetAsync(raffleurl, ct);
      if(!getRaffle.IsSuccessStatusCode) throw new InvalidOperationException("Can't access site");
      
      var finalHtml = await getRaffle.Content.ReadAsStringAsync(ct);
      var raffleDoc = new HtmlDocument();
      raffleDoc.LoadHtml(finalHtml);

      var formidhtml = raffleDoc.DocumentNode.SelectSingleNode("//input[@name='nome_campagna']");
      var formid = formidhtml.GetAttributeValue("value", "");

      return formid;
    }

    public async Task<bool> SubmitAsync(SOTFSubmitPayload payload, CancellationToken ct)
    {
      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"contatto_lang", "en"},
        {"nome_campagna", payload.FormId},
        {"submitted", "true"},
        {"reg_form_name", payload.Profile.ShippingAddress.FirstName},
        {"reg_form_surname", payload.Profile.ShippingAddress.LastName},
        {"reg_form_instagram_profile", payload.InstagramHandle},
        {"reg_form_email", payload.Email},
        {"reg_form_size", payload.SizeValue},
        {"reg_form_color", payload.Colourway},
        {"g-recaptcha-response", payload.Captcha},
        {"button", "send"}
      });


      var endpoint = "https://www.sotf.com/en/raffle_online_form.php";
      var signup = await HttpClient.PostAsync(endpoint, content, ct);
      return signup.IsSuccessStatusCode;
    }
  }
}