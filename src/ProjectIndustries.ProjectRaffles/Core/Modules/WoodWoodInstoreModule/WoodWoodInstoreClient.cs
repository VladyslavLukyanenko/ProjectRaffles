using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.WoodWoodInstoreModule
{
  public class WoodWoodInstoreClient : ModuleHttpClientBase, IWoodWoodInstoreClient
  {
    private readonly IPhoneCodeService _phoneCodeService;

    public WoodWoodInstoreClient(IPhoneCodeService phoneCodeService)
    {
      _phoneCodeService = phoneCodeService;
    }

    protected override void ConfigureHttpClient(HttpClientOptions options)
    {
      options.AllowAutoRedirect = false;
      options.PostConfigure = httpClient =>
      {
        httpClient.DefaultRequestHeaders.Add("User-Agent",
          "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
        httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        httpClient.DefaultRequestHeaders.Add("dnt", "1");
        httpClient.DefaultRequestHeaders.Add("accept",
          "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
        httpClient.DefaultRequestHeaders.Add("accept-language", "en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "cross-site");
        httpClient.DefaultRequestHeaders.Add("sec-gpc", "1");
      };
    }

    public async Task<string> GetIPAsync(CancellationToken ct)
    {
      var ipresponse = await HttpClient.GetAsync("http://bot.whatismyipaddress.com/", ct);
      var publicIp = await ipresponse.ReadStringResultOrFailAsync("Can't get IP", ct);

      return publicIp;
    }

    public async Task<string> GetPhoneCodeAsync(string countryId, CancellationToken ct)
    {
      var phoneCode = await _phoneCodeService.GetPhoneCodeAsync(countryId, ct);

      return phoneCode;
    }

    public async Task<bool> SubmitAsync(WoodWoodInstorePayload payload, CancellationToken ct)
    {
      var phoneNumber = "+" + payload.PhoneCode + payload.Address.PhoneNumber.Value;
      
      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"tags[]", payload.ProductTag},
        {"token", "4471020e-81f0638-c2d1963-04b2c92-15e0283-a85"},
        {"rule_email", payload.Email},
        {"fields[Raffle.Phone Number]", phoneNumber},
        {"fields[Raffle.First Name]", payload.Address.FirstName.Value},
        {"fields[Raffle.Last Name]", payload.Address.LastName.Value},
        {"fields[Raffle.Shipping Address]", payload.Address.AddressLine1.Value},
        {"fields[Raffle.Postal Code]", payload.Address.PostCode.Value},
        {"fields[Raffle.City]", payload.Address.City.Value},
        {"fields[Raffle.Country]", payload.Address.CountryId.Value}, //2 digits
        {"fields[Raffle.Size]", payload.Size},
        {"fields[Raffle.Collect]", payload.Store}, //2 digits
        {"fields[SignupSource.ip]", payload.Ip},
        {"fields[SignupSource.useragent]", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36"},
        {"language", "sv"},
        {"g-recaptcha-response", payload.Captcha}
      });

      HttpClient.DefaultRequestHeaders.Add("Origin","https://www.woodwood.com");
      HttpClient.DefaultRequestHeaders.Add("Referer","https://www.woodwood.com/");
      
      var url = "https://app.rule.io/subscriber-form/subscriber";
      var signup = await HttpClient.PostAsync(url, content, ct);

      string headerLocation = "";
      var headers = signup.Headers;
      IEnumerable<string> values;
      if (headers.TryGetValues("location", out values))
      { 
        headerLocation = values.First();
      }

      if (!headerLocation.Equals("https://www.woodwood.com/en/raffle-confirm-email"))
        await signup.FailWithRootCauseAsync("Error on submission", ct);
      
      return headerLocation.Equals("https://www.woodwood.com/en/raffle-confirm-email"); //we check headers for success because site requires CF cookies to access
    }
  }
}