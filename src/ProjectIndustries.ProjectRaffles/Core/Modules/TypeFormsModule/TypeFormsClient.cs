using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TypeFormsModule
{
  public class TypeFormsClient : ModuleHttpClientBase, ITypeFormsClient
  {
    private readonly CookieContainer _cookieContainer = new CookieContainer();

    protected override void ConfigureHttpClient(HttpClientOptions options)
    {
      options.CookieContainer = _cookieContainer;
      options.PostConfigure = httpClient =>
      {
        httpClient.DefaultRequestHeaders.Add("User-Agent",
          "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
        httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        httpClient.DefaultRequestHeaders.Add("dnt", "1");
      };
    }

    public async Task<TypeFormsSubmission> StartSubmissionAsync(string url, CancellationToken ct)
    {
      //get cookies
      var attributionCookie = new Cookie("attribution_user_id", Guid.NewGuid().ToString()) {Domain = "typeform.com"};
      _cookieContainer.Add(attributionCookie);


      var submissionUrl = url.Replace("typeform.com/to/", "typeform.com/forms/") +
                          "/start-submission";

      var getBody = await HttpClient.PostAsync(submissionUrl, null, ct);
      if (!getBody.IsSuccessStatusCode) await getBody.FailWithRootCauseAsync("Can't start submission process", ct);

      var body = await getBody.Content.ReadAsStringAsync(ct);

      dynamic signatureParsed = JObject.Parse(body);
      string signatureValue = (string) signatureParsed["signature"];

      string formId = (string) signatureParsed["submission"]["form_id"];

      int landed = (int) signatureParsed["submission"]["landed_at"];

      return new TypeFormsSubmission(signatureValue, landed, formId);
    }

    public async Task<bool> SubmitAsync(string sourceUrl, TypeFormsSubmitPayload payload, CancellationToken ct)
    {
      var baseContent = JsonConvert.SerializeObject(payload);
      var content = new StringContent(baseContent, Encoding.UTF8, "application/json");

      var uri = new Uri(sourceUrl);
      var uriBase = uri.Host;
      
      var endpoint = sourceUrl.Replace("typeform.com/to/", "typeform.com/forms/") +
                     "/complete-submission";

      
      HttpClient.DefaultRequestHeaders.Add("origin", "https://"+ uriBase);
      HttpClient.DefaultRequestHeaders.Add("referer", sourceUrl);
      var postContent = await HttpClient.PostAsync(endpoint, content, ct);
      if (!postContent.IsSuccessStatusCode)
      {
        await postContent.FailWithRootCauseAsync("Error on submission", ct);
      }

      return true;
    }
  }
}