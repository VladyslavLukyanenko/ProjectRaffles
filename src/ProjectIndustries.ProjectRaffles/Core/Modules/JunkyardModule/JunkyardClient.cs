using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.JunkyardModule
{
  public class JunkyardClient : ModuleHttpClientBase, IJunkyardClient
  {
    private readonly CookieContainer _cookieContainer = new CookieContainer();

    protected override void ConfigureHttpClient(HttpClientOptions options)
    {
      options.AllowAutoRedirect = false;
      options.CookieContainer = _cookieContainer;
      options.PostConfigure = httpClient =>
      {
        httpClient.DefaultRequestHeaders.Add("user-agent",
          "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
        httpClient.DefaultRequestHeaders.Add("dnt", "1");
        httpClient.DefaultRequestHeaders.Add("Accept","application/json, text/plain, */*");
        httpClient.DefaultRequestHeaders.Add("Accept-Language","en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
        httpClient.DefaultRequestHeaders.Add("Connection","keep-alive");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest","empty");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode","cors");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site","same-origin");
        httpClient.DefaultRequestHeaders.Add("sec-gpc","1");
      };
    }
    
    public async Task<string> GetProductAsync(string url, CancellationToken ct)
    {
      var getPage = await HttpClient.GetAsync(url, ct);
      var body = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

      var doc = new HtmlDocument();
      doc.LoadHtml(body);

      var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;

      return node;
    }

    public async Task<JunkyardParsedRaffleFields> ParseRaffleAsync(string size, string raffleurl, CancellationToken ct)
    {
      var getPage = await HttpClient.GetAsync(raffleurl, ct);
      var body = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

      var doc = new HtmlDocument();
      doc.LoadHtml(body);

      //get general raffle details
      var formKey = doc.DocumentNode.SelectSingleNode("//input[@name='form_key']").GetAttributeValue("value", "");
      var raffleId = doc.DocumentNode.SelectSingleNode("//input[@name='raffle_id']").GetAttributeValue("value", "");

      //find size
      var pattern = @"<option value="".*"">" + size + @"<\/option>";
      Regex sizeRegex = new Regex(pattern);
      var match = sizeRegex.Match(body).ToString();
      var sizeValue = match.Replace("<option value=\"", "").Replace($"\">{size}</option>", "");
      
      if(sizeValue == null) throw new InvalidOperationException("Can't find size!");

      //find the name of the "botField"
      var botPattern = @"<input type=""hidden"" name="".{32}"" v";
      Regex botRegex = new Regex(botPattern);
      var botMatch = botRegex.Match(body).ToString();
      var botField = botMatch.Replace(@"<input type=""hidden"" name=""", "").Replace(@""" v", "");

      //find the value of the "botField"
      var botValuePattern = @"value="".{32}"" \/>";
      Regex botValueRegex = new Regex(botValuePattern);
      var botFieldValue = botValueRegex.Match(body).ToString();
      var botValue = botFieldValue.Replace(@"value=""", "").Replace(@""" />", "");

      return new JunkyardParsedRaffleFields(formKey, raffleId, sizeValue, botField, botValue);
    }


    public async Task<bool> SubmitAsync(JunkyardSubmitPayload payload, CancellationToken ct)
    {
      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"token", payload.Captcha},
        {"action", "submit_raffleform"},
        {"form_key", payload.ParsedRaffle.FormKey},
        {"raffle_id", payload.ParsedRaffle.RaffleId},
        {payload.ParsedRaffle.BotField, payload.ParsedRaffle.BotValue}, //their "botprotection"
        {"size", payload.SizeValue},
        {"customer_email", payload.Email}
      });


      var uri = new Uri(payload.RaffleUrl);
      var baseurl = uri.Host;
      
      HttpClient.DefaultRequestHeaders.Add("Origin",$"https://{baseurl}");
      HttpClient.DefaultRequestHeaders.Add("Referer",payload.RaffleUrl);
      
      HttpClient.DefaultRequestHeaders.Add("x-prototype-version","1.7");
      HttpClient.DefaultRequestHeaders.Add("x-requested-with","XMLHttpRequest");
      
      var postUrl = $"https://{baseurl}/junkraffle/raffle/submit";
      var signup = await HttpClient.PostAsync(postUrl, content, ct);
      var signupBody = await signup.ReadStringResultOrFailAsync($"Error on submission with email: {payload.Email}", ct);

      if (!signupBody.Contains("Woohoo"))
        await signup.FailWithRootCauseAsync($"Error on submission with email: {payload.Email}", ct);

      return signupBody.Contains("Woohoo");
    }
  }
}