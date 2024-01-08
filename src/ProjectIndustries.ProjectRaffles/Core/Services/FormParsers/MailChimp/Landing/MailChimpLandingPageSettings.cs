using System;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing
{
  public class MailChimpLandingPageSettings
  {
    [JsonProperty("honeypotFieldName")] public string HoneypotFieldName { get; set; }
    [JsonProperty("honeytime")] public string HoneyTime { get; set; }
    public string SubscribeUrl { get; set; }
    public string RecaptchaSitekey { get; set; }
    public bool CaptchaEnabled { get; set; }
    public SettingsConfig Config { get; set; }

    public Uri GetNormalizedSubmitUrl()
    {
      var rawSubmitUrl = SubscribeUrl;
      if (!rawSubmitUrl.StartsWith("http"))
      {
        rawSubmitUrl = "https:" + rawSubmitUrl;
      }

      var builder = new UriBuilder(rawSubmitUrl)
      {
        Query = string.Empty
      };

      return builder.Uri;
    }
  }
}