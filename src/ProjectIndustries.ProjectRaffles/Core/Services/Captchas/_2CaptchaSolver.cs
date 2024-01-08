using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Captchas
{
  [Obfuscation(Feature = "when anonymous: renaming", Exclude = true, ApplyToMembers = true)]
  public class _2CaptchaSolver : ICaptchaSolver
  {
    private string _apiUrl = "https://2captcha.com/";

    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    private class _2CaptchaResultInternal
    {
      public int Status { get; set; }
      public string Request { get; set; }
    }

    public _2CaptchaSolver(string apiKey, HttpClient httpClient = null)
    {
      _httpClient = httpClient ?? new HttpClient();
      _apiKey = apiKey;
    }

    //docs: https://2captcha.com/2captcha-api#intro
    public async Task<decimal> GetBalance(CancellationToken ct)
    {
      var getData = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"key", _apiKey},
        {"action", "getbalance"},
        {"json", "1"}
      });

      var inResponse = await _httpClient.PostAsync(_apiUrl + "res.php", getData, ct);
      var inJson = await inResponse.Content.ReadAsStringAsync(ct);

      var @in = JsonConvert.DeserializeObject<_2CaptchaResultInternal>(inJson);
      if (@in.Status == 0 || !decimal.TryParse(@in.Request, out var balance))
      {
        throw new InvalidOperationException("Can't get balance");
      }

      return balance;
    }

    public async Task<CaptchaResult> SolveImageCaptchaAsync(string base64Image, CancellationToken ct)
    {
      var imageContent = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"key", _apiKey},
        {"action", "base64"},
        {"body", base64Image},
        {"json", "1"}
      });

      var inResponse = await _httpClient.PostAsync(_apiUrl + "in.php", imageContent, ct);
      var inJson = await inResponse.Content.ReadAsStringAsync(ct);

      var @in = JsonConvert.DeserializeObject<_2CaptchaResultInternal>(inJson);
      if (@in.Status == 0)
      {
        return new CaptchaResult(false, @in.Request);
      }

      await Task.Delay(5 * 1000, ct); //wait 5 seconds before trying to get response
      return await GetResponse(@in.Request, ct);
    }

    public async Task<CaptchaResult> SolveReCaptchaV2Async(string sitekey, string pageUrl, bool isVisible,
      CancellationToken ct)
    {
      var reCaptchaContent = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"key", _apiKey},
        {"method", "userrecaptcha"},
        {"googlekey", sitekey},
        {"pageurl", pageUrl},
        {"invisible", isVisible ? "1" : "0"}, //needs to be "1" or "0"
        {"json", "1"}
      });

      var inResponse = await _httpClient.PostAsync(_apiUrl + "in.php", reCaptchaContent, ct);
      var inJson = await inResponse.Content.ReadAsStringAsync(ct);

      var @in = JsonConvert.DeserializeObject<_2CaptchaResultInternal>(inJson);
      if (@in.Status == 0)
      {
        return new CaptchaResult(false, @in.Request);
      }

      await Task.Delay(15 * 1000, ct); //wait 15 seconds before trying to get response, recaptcha solving takes longer
      return await GetResponse(@in.Request, ct);
    }
    
    public async Task<CaptchaResult> SolveReCaptchaV3Async(string sitekey, string pageUrl, string action, double minScore, CancellationToken ct)
    {
      var reCaptchaContent = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"key", _apiKey},
        {"method", "userrecaptcha"},
        {"googlekey", sitekey},
        {"pageurl", pageUrl},
        {"action",  action}, 
        {"min_score",  minScore.ToString(CultureInfo.InvariantCulture)}, 
        {"json", "1"}
      });

      var inResponse = await _httpClient.PostAsync(_apiUrl + "in.php", reCaptchaContent, ct);
      var inJson = await inResponse.Content.ReadAsStringAsync(ct);

      var @in = JsonConvert.DeserializeObject<_2CaptchaResultInternal>(inJson);
      if (@in.Status == 0)
      {
        return new CaptchaResult(false, @in.Request);
      }

      await Task.Delay(15 * 1000, ct); //wait 15 seconds before trying to get response, recaptcha solving takes longer
      return await GetResponse(@in.Request, ct);
    }

    private async Task<CaptchaResult> GetResponse(string solveId, CancellationToken ct)
    {
      var apiKeySafe = Uri.EscapeUriString(_apiKey);

      //run loop until response is there
      while (true)
      {
        var resJson =
          await _httpClient.GetStringAsync(_apiUrl + $"res.php?key={apiKeySafe}&id={solveId}&action=get&json=1");

        var res = JsonConvert.DeserializeObject<_2CaptchaResultInternal>(resJson);
        if (res.Status == 0)
        {
          if (res.Request == "ERROR_NO_SLOT_AVAILABLE")
          {
            throw new InvalidOperationException("2Captcha queue is full!");
          }

          if (res.Request == "CAPCHA_NOT_READY") //it's supposed to be like this
          {
            await Task.Delay(5 * 1000, ct);
            continue;
          }

          return new CaptchaResult(false, res.Request);
        }

        return new CaptchaResult(true, res.Request);
      }
    }
  }
}