using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.ShopifyAccountGenerator
{
  public class ShopifyAccountGeneratorClient : IShopifyAccountGeneratorClient
  {
    private readonly ICaptchaSolveService _captchaSolver;
    private readonly CookieContainer _cookieContainer = new CookieContainer();
    private readonly IHttpClientBuilder _builder;
    private HttpClient _httpClient;

    public ShopifyAccountGeneratorClient(ICaptchaSolveService captchaSolver, IHttpClientBuilder builder)
    {
      _builder = builder;
      _captchaSolver = captchaSolver;
    }
    
    public void GenerateHttpClient()
    {
      _httpClient = _builder.WithConfiguration(ConfigureHttpClient)
        .Build();
    }

    private void ConfigureHttpClient(HttpClientOptions options)
    {
      options.CookieContainer = _cookieContainer;
      options.AllowAutoRedirect = true;
      options.PostConfigure = httpClient =>
      {
        httpClient.DefaultRequestHeaders.Add("User-Agent",
          "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
        httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        httpClient.DefaultRequestHeaders.Add("dnt", "1");
      };
    }

    public async Task<string> PostAccountInfoAsync(string baseurl, string firstname, string lastname, string email,
      CancellationToken ct)
    {
      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"form_type", "create_customer"},
        {"utf8", "✓"},
        {"customer[first_name]", firstname},
        {"customer[last_name]", lastname},
        {"customer[email]", email},
        {"customer[password]", "ProjectRaffles!1)3!"}
      });

      _httpClient.DefaultRequestHeaders.Add("origin", baseurl);
      _httpClient.DefaultRequestHeaders.Add("referer", baseurl);
      var endpoint = "https://" + baseurl + "/account";
      var postAccount = await _httpClient.PostAsync(endpoint, content, ct);
      var body = await postAccount.ReadStringResultOrFailAsync("Failed on submission (1)");

      _httpClient.DefaultRequestHeaders.Remove("referer");
      return body;
    }

    public async Task<bool> PostCaptchaAsync(string baseurl, string body, CancellationToken ct)
    {
      if (!body.Contains("challenge")) return true;

      var doc = new HtmlDocument();
      doc.LoadHtml(body);

      var authKey = doc.DocumentNode.SelectSingleNode("//input[@name='authenticity_token']")
        .GetAttributeValue("value", "");

      var captchaRegex = new Regex(@"sitekey: "".*""");
      var findCaptchaMatch = captchaRegex.Match(body).ToString();
      var captchaSiteKey = findCaptchaMatch.Replace("sitekey: ", "").Replace(@"""", "");

      var captchaUrl = "https://" + baseurl + "/challenge";
      var captcha = await _captchaSolver.SolveReCaptchaV2Async(captchaSiteKey, captchaUrl, true, ct);

      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"authenticity_token", authKey},
        {"g-recaptcha-response", captcha}
      });

      var referer = baseurl + "/challenge";
      _httpClient.DefaultRequestHeaders.Remove("referer");
      _httpClient.DefaultRequestHeaders.Add("referer", referer);
      var endpoint = "https://" + baseurl + "/account";
      var postAccount = await _httpClient.PostAsync(endpoint, content, ct);
      if (!postAccount.IsSuccessStatusCode) await postAccount.FailWithRootCauseAsync("Failed on submission (2)");

      return postAccount.IsSuccessStatusCode;
    }
  }
}