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
  public class CapmonsterCloudSolver : ICaptchaSolver
  {
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    private class CapmonsterCloudResultInternal
    {
      public bool Success { get; set; }
      public string Response { get; set; }
      public Dictionary<string, object> Dictionary { get; set; }

      public CapmonsterCloudResultInternal(bool success, string response, Dictionary<string, object> dictionary)
      {
        Success = success;
        Response = response;
        Dictionary = dictionary;
      }
    }

    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    private class CapmonsterCloudApiResponse
    {
      public int ErrorId { get; set; }
      public string ErrorCode { get; set; }
      public int TaskId { get; set; }
      public string Status { get; set; }
      public Dictionary<string, object> Solution { get; set; }
      public decimal Balance { get; set; }
    }

    private class CapmonsterCloudCaptchaContent
    {
      [JsonProperty("clientKey")] public string ClientKey { get; set; }
      [JsonProperty("task")] public CapmonsterCloudCaptchaTask Task { get; set; }
    }

    private class CapmonsterCloudCaptchaTask
    {
      [JsonProperty("type")] public string Type { get; set; }
      [JsonProperty("body")] public string Body { get; set; }
      [JsonProperty("websiteURL")] public string WebsiteURL { get; set; }
      [JsonProperty("websiteKey")] public string WebsiteKey { get; set; }
      [JsonProperty("isInvisible")] public bool IsInvisible { get; set; }
      [JsonProperty("minScore")] public double? MinScore { get; set; }
      [JsonProperty("pageAction")] public string PageAction { get; set; }
    }

    private string _apiUrl = "https://api.capmonster.cloud/";

    public CapmonsterCloudSolver(string apiKey, HttpClient httpClient = null)
    {
      _httpClient = httpClient ?? new HttpClient();
      _apiKey = apiKey;
    }

    //docs: https://zennolab.atlassian.net/wiki/spaces/APIS/overview
    public async Task<decimal> GetBalance(CancellationToken ct = default)
    {
      var contentRoot = new CapmonsterCloudCaptchaContent
      {
        ClientKey = _apiKey
      };

      var content = JsonConvert.SerializeObject(contentRoot);
      var contentJson = new StringContent(content, Encoding.UTF8, "application/json");

      var inResponse = await _httpClient.PostAsync(_apiUrl + "getBalance", contentJson, ct);
      var inJson = await inResponse.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

      var @in = JsonConvert.DeserializeObject<CapmonsterCloudApiResponse>(inJson);
      if (@in.ErrorId != 0)
      {
        throw new InvalidOperationException("Can't get balance. Error: " + @in.ErrorCode);
      }

      return @in.Balance;
    }

    public async Task<CaptchaResult> SolveImageCaptchaAsync(string base64image, CancellationToken ct)
    {
      var imageContent = new CapmonsterCloudCaptchaContent
      {
        ClientKey = _apiKey,
        Task = new CapmonsterCloudCaptchaTask
        {
          Type = "ImageToTextTask",
          Body = base64image
        }
      };

      var content = JsonConvert.SerializeObject(imageContent);
      var contentJson = new StringContent(content, Encoding.UTF8, "application/json");


      var inResponse = await _httpClient.PostAsync(_apiUrl + "createTask", contentJson, ct);
      var inJson = await inResponse.Content.ReadAsStringAsync(ct);

      var @in = JsonConvert.DeserializeObject<CapmonsterCloudApiResponse>(inJson);
      if (@in.ErrorId != 0)
      {
        return new CaptchaResult(false, @in.ErrorCode);
      }

      await Task.Delay(2 * 1000,
        ct); //2 second delay before trying to get result, they claim it will come within 300ms-6s so 2s fits good
      var result = await GetResponse(@in.TaskId, ct);

      if (!result.Success)
        return new CaptchaResult(false, result.Response);

      return new CaptchaResult(true, result.Dictionary["text"].ToString());
    }

    public async Task<CaptchaResult> SolveReCaptchaV2Async(string sitekey, string siteurl, bool isVisible,
      CancellationToken ct = default)
    {
      var captchaContent = new CapmonsterCloudCaptchaContent
      {
        ClientKey = _apiKey,
        Task = new CapmonsterCloudCaptchaTask
        {
          Type = "NoCaptchaTaskProxyless", //we can also do with proxy, then we just gotta supply it along, will use users' proxy data
          WebsiteURL = siteurl,
          WebsiteKey = sitekey
        }
      };

      var content = JsonConvert.SerializeObject(captchaContent);
      var contentJson = new StringContent(content, Encoding.UTF8, "application/json");


      var inResponse = await _httpClient.PostAsync(_apiUrl + "createTask", contentJson, ct);
      var inJson = await inResponse.Content.ReadAsStringAsync(ct);

      var @in = JsonConvert.DeserializeObject<CapmonsterCloudApiResponse>(inJson);
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
      double minScore, CancellationToken ct)
    {
      var captchaContent = new CapmonsterCloudCaptchaContent
      {
        ClientKey = _apiKey,
        Task = new CapmonsterCloudCaptchaTask
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

      var @in = JsonConvert.DeserializeObject<CapmonsterCloudApiResponse>(inJson);
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

    private async Task<CapmonsterCloudResultInternal> GetResponse(int taskId, CancellationToken ct = default)
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

        var res = JsonConvert.DeserializeObject<CapmonsterCloudApiResponse>(responseJson);
        if (res.ErrorId != 0)
        {
          return new CapmonsterCloudResultInternal(false, res.ErrorCode, null);
        }

        if (res.Status == "processing")
        {
          await Task.Delay(5 * 1000, ct).ConfigureAwait(false);
          continue;
        }

        return new CapmonsterCloudResultInternal(true, null, res.Solution);
      }
    }
  }
}