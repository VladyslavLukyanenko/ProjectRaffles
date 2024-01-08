using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing
{
  public class MailChimpLandingPageFormParser : IMailChimpLandingPageFormParser
  {
    private readonly IEnumerable<IMailChimpFieldFactory> _factories;
    private readonly ICaptchaSolveService _captchaSolveService;

    public MailChimpLandingPageFormParser(IEnumerable<IMailChimpFieldFactory> factories,
      ICaptchaSolveService captchaSolveService)
    {
      _factories = factories;
      _captchaSolveService = captchaSolveService;
    }

    public async ValueTask<MailChimpParseResult> ParseAsync(Uri fieldsConfigUrl, HttpClient client,
      CancellationToken ct = default)
    {
      var message = new HttpRequestMessage(HttpMethod.Get, fieldsConfigUrl);
      var response = await client.SendAsync(message, ct);
      string rawJson = await response.Content.ReadPossiblyGZippedAsStringAsync(ct);

      var json = JsonConvert.DeserializeObject<MailChimpLandingPageSettings>(rawJson);
      var query = QueryStringUtil.Parse(fieldsConfigUrl.Query);
      var fields = new List<Field>();

      var u = query["u"].FirstOrDefault();
      var id = query["id"].FirstOrDefault();
      if (!string.IsNullOrEmpty(u))
      {
        fields.Add(new HiddenField("u", u));
      }

      if (!string.IsNullOrEmpty(id))
      {
        fields.Add(new HiddenField("id", id));
      }

      foreach (var settingsField in json.Config.Fields)
      {
        IMailChimpFieldFactory factory = _factories.FirstOrDefault(_ => _.IsSupports(settingsField.Type))
                                         ?? throw new InvalidOperationException(
                                           $"Field type '{settingsField.Type}' is not supported.");

        var mailChimpField = factory.Create(settingsField);
        fields.AddRange(mailChimpField.ConvertToFields());
      }

      fields.Add(new HiddenField(json.HoneypotFieldName, null));
      fields.Add(new HiddenField("ht", json.HoneyTime));
      fields.Add(new HiddenField("c", "dojo_request_script_callbacks.dojo_request_script2"));

      // var domain = fieldsConfigUrl.GetLeftPart(UriPartial.Authority);
      // var submitUrlBuilder = new UriBuilder(domain)
      // {
      // Path = "/signup-form/subscribe"
      // };

      return new MailChimpParseResult(fields, new LandingPageFormSubmitHandler(_captchaSolveService, json, client));
    }
  }
}