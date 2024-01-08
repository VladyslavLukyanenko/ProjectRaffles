using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.ViralSweep.FieldFactories;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.ViralSweep
{
  public class ViralSweepSubmitHandler : IFormSubmitHandler
  {
    private readonly HttpClient _client;
    private readonly string _submitMethod;

    public ViralSweepSubmitHandler(HttpClient client, string submitMethod)
    {
      _client = client;
      _submitMethod = submitMethod;
    }

    public async ValueTask<FormSubmitResult> SubmitAsync(Uri pageUrl, IEnumerable<Field> fields,
      CancellationToken ct = default)
    {
      var queryParams = fields
        .Where(f => !(f is ViralSweepMultiCheckboxField) && (!(f is CheckboxField cb) || cb.IsChecked))
        .Select(_ => new KeyValuePair<string, string>(_.SystemName, _.Value?.ToString()))
        .ToList();

      var submitPayload = new FormUrlEncodedContent(queryParams);
      var message = new HttpRequestMessage(new HttpMethod(_submitMethod), "https://app.viralsweep.com/promo/enter")
      {
        Content = submitPayload
      };

      var postAccount = await _client.SendAsync(message, ct);
      string content = await postAccount.Content.ReadPossiblyGZippedAsStringAsync(ct);
      var json = JObject.Parse(content);

      var result = json.TryGetValue("success", out var successProp) && successProp.Value<int>() == 1;

      return result
        ? FormSubmitResult.Successful()
        : FormSubmitResult.Failed(content);
    }
  }
}