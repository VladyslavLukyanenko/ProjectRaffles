using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Captchas
{
  [Obfuscation(Feature = "when anonymous: renaming", Exclude = true, ApplyToMembers = true)]
  public class AntiCaptchaSolver : ICaptchaSolver
  {
    private string _apiUrl = "https://api.anti-captcha.com/";

    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    private class AntiCaptchaResultInternal
    {
      public bool Success { get; set; }
      public string Response { get; set; }
      public Dictionary<string, object> Dictionary { get; set; }

      public AntiCaptchaResultInternal(bool success, string response, Dictionary<string, object> dictionary)
      {
        Success = success;
        Response = response;
        Dictionary = dictionary;
      }
    }

    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    private class AntiCaptchaApiResponse
    {
      public int ErrorId { get; set; }
      public string ErrorCode { get; set; }
      public int TaskId { get; set; }
      public string Status { get; set; }
      public Dictionary<string, object> Solution { get; set; }
      public decimal Balance { get; set; }
    }

    private class AntiCaptchaContent
    {
      [JsonProperty("clientKey")] public string ClientKey { get; set; }
      [JsonProperty("task")] public AntiCaptchaTask Task { get; set; }
    }

    private class AntiCaptchaTask
    {
      [JsonProperty("type")] public string Type { get; set; }
      [JsonProperty("body")] public string Body { get; set; }
      [JsonProperty("websiteURL")] public string WebsiteURL { get; set; }
      [JsonProperty("websiteKey")] public string WebsiteKey { get; set; }
      [JsonProperty("isInvisible")] public bool IsInvisible { get; set; }
      [JsonProperty("minScore")] public double? MinScore { get; set; }
      [JsonProperty("pageAction")] public string PageAction { get; set; }
    }

    public AntiCaptchaSolver(string apiKey, HttpClient httpClient = null)
    {
      _httpClient = httpClient ?? new HttpClient();
      _apiKey = apiKey;
    }

    //docs: https://anticaptcha.atlassian.net/wiki/spaces/API/pages/578322433/API+Methods
    public async Task<decimal> GetBalance(CancellationToken ct)
    {
      var content = new AntiCaptchaContent
      {
        ClientKey = _apiKey
      };

      var contentJson = JsonConvert.SerializeObject(content);
      var inResponse = await _httpClient.PostAsync(_apiUrl + "getBalance", new StringContent(contentJson), ct);
      var inJson = await inResponse.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

      var @in = JsonConvert.DeserializeObject<AntiCaptchaApiResponse>(inJson);
      if (@in.ErrorId != 0)
      {
        throw new InvalidOperationException("Can't get balance. Error: " + @in.ErrorCode);
      }

      return @in.Balance;
    }

    public async Task<CaptchaResult> SolveImageCaptchaAsync(string base64image, CancellationToken ct)
    {
      var imageContent = new AntiCaptchaContent
      {
        ClientKey = _apiKey,
        Task = new AntiCaptchaTask
        {
          Type = "ImageToTextTask",
          Body = base64image
        }
      };

      var content = JsonConvert.SerializeObject(imageContent);
      var contentJson = new StringContent(content, Encoding.UTF8, "application/json");


      var inResponse = await _httpClient.PostAsync(_apiUrl + "createTask", contentJson, ct);
      var inJson = await inResponse.Content.ReadAsStringAsync(ct);

      var @in = JsonConvert.DeserializeObject<AntiCaptchaApiResponse>(inJson);
      if (@in.ErrorId != 0)
      {
        return new CaptchaResult(false, @in.ErrorCode);
      }

      await Task.Delay(5 * 1000, ct); //5 second delay before trying to get result
      var result = await GetResponse(@in.TaskId, ct);

      if (!result.Success)
        return new CaptchaResult(false, result.Response);

      return new CaptchaResult(true, result.Dictionary["text"].ToString());
    }

    public async Task<CaptchaResult> SolveReCaptchaV2Async(string sitekey, string siteurl, bool isVisible,
      CancellationToken ct = default)
    {
      var captchaContent = new AntiCaptchaContent
      {
        ClientKey = _apiKey,
        Task = new AntiCaptchaTask
        {
          Type = "NoCaptchaTaskProxyless", //we can also do with proxy, then we just gotta supply it along, will use users' proxy data, according to their docs, there's no difference
          WebsiteURL = siteurl,
          WebsiteKey = sitekey,
          IsInvisible = isVisible
        }
      };

      var content = JsonConvert.SerializeObject(captchaContent);
      var contentJson = new StringContent(content, Encoding.UTF8, "application/json");


      var inResponse = await _httpClient.PostAsync(_apiUrl + "createTask", contentJson, ct);
      var inJson = await inResponse.Content.ReadAsStringAsync(ct);

      var @in = JsonConvert.DeserializeObject<AntiCaptchaApiResponse>(inJson);
      if (@in.ErrorId != 0)
      {
        return new CaptchaResult(false, @in.ErrorCode);
      }

      await Task.Delay(15 * 1000, ct); //15 second delay before trying to get result
      var result = await GetResponse(@in.TaskId, ct);

      if (!result.Success)
        return new CaptchaResult(false, result.Response);

      return new CaptchaResult(true, result.Dictionary["gRecaptchaResponse"].ToString());
    }

    public async Task<CaptchaResult> SolveReCaptchaV3Async(string sitekey, string siteurl, string action,
      double minScore, CancellationToken ct = default)
    {
      var captchaContent = new AntiCaptchaContent
      {
        ClientKey = _apiKey,
        Task = new AntiCaptchaTask
        {
          Type = "RecaptchaV3TaskProxyless",
          WebsiteURL = siteurl,
          WebsiteKey = sitekey,
          MinScore = minScore,
          PageAction = action
        }
      };

      var content = JsonConvert.SerializeObject(captchaContent);
      var contentJson = new StringContent(content, Encoding.UTF8, "application/json");


      var inResponse = await _httpClient.PostAsync(_apiUrl + "createTask", contentJson, ct);
      var inJson = await inResponse.Content.ReadAsStringAsync(ct);

      var @in = JsonConvert.DeserializeObject<AntiCaptchaApiResponse>(inJson);
      if (@in.ErrorId != 0)
      {
        return new CaptchaResult(false, @in.ErrorCode);
      }

      await Task.Delay(15 * 1000, ct); //15 second delay before trying to get result
      var result = await GetResponse(@in.TaskId, ct);

      if (!result.Success)
        return new CaptchaResult(false, result.Response);

      return new CaptchaResult(true, result.Dictionary["gRecaptchaResponse"].ToString());
    }

    private async Task<AntiCaptchaResultInternal> GetResponse(int taskId, CancellationToken ct = default)
    {
      var content = new Dictionary<string, object>
      {
        {"clientKey", _apiKey},
        {"taskId", taskId},
      };

      var contentJson = JsonConvert.SerializeObject(content);

      while (true)
      {
        var response = await _httpClient
          .PostAsync(_apiUrl + "getTaskResult", new StringContent(contentJson), ct)
          .ConfigureAwait(false);
        var responseJson = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

        var res = JsonConvert.DeserializeObject<AntiCaptchaApiResponse>(responseJson);
        if (res.ErrorId != 0)
        {
          return new AntiCaptchaResultInternal(false, res.ErrorCode, null);
        }

        if (res.Status == "processing")
        {
          await Task.Delay(5 * 1000, ct).ConfigureAwait(false);
          continue;
        }

        return new AntiCaptchaResultInternal(true, null, res.Solution);
      }
    }
  }
}