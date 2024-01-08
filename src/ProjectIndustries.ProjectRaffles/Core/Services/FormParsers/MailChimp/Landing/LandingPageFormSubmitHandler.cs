using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing
{
  public class LandingPageFormSubmitHandler : IFormSubmitHandler
  {
    private readonly ICaptchaSolveService _captchaSolveService;
    private readonly MailChimpLandingPageSettings _settings;
    private readonly HttpClient _httpClient;

    public LandingPageFormSubmitHandler(ICaptchaSolveService captchaSolveService, MailChimpLandingPageSettings settings,
      HttpClient httpClient)
    {
      _captchaSolveService = captchaSolveService;
      _settings = settings;
      _httpClient = httpClient;
    }

    public async ValueTask<FormSubmitResult> SubmitAsync(Uri pageUrl, IEnumerable<Field> fields,
      CancellationToken ct = default)
    {
      var submitFields = fields.ToList();

      if (_settings.CaptchaEnabled)
      {
        var solvedToken =
          await _captchaSolveService.SolveReCaptchaV2Async(_settings.RecaptchaSitekey, pageUrl.ToString(), true, ct);
        submitFields.Add(new HiddenField("g-recaptcha-response", solvedToken));
      }

      var queryParams = submitFields.Where(f => !(f is CheckboxField cb) || cb.IsChecked)
        .Select(_ =>
          new KeyValuePair<string, string>(_.SystemName.UriDataEscape(), _.Value?.ToString().UriDataEscape()))
        .ToList();

      var submitUrl = _settings.GetNormalizedSubmitUrl();
      var host = submitUrl.GetLeftPart(UriPartial.Authority);
      var pathFormat =
        "{0}/signup-form/track-submit?u={1}&id={2}&c=dojo_request_script_callbacks.dojo_request_script1";

      var uField = submitFields.First(_ => _.SystemName == "u");
      var idField = submitFields.First(_ => _.SystemName == "id");

      var url = string.Format(pathFormat, host, uField.Value, idField.Value);
      await _httpClient.GetStringAsync(url, ct);

      var query = string.Join("&", queryParams.Select(p => $"{p.Key}={p.Value}"));
      var submitUrlBuilder = new UriBuilder(submitUrl) {Query = query};

      var postAccount = await _httpClient.GetAsync(submitUrlBuilder.Uri, ct);
      var content = await postAccount.Content.ReadPossiblyGZippedAsStringAsync(ct);

      var result = content.Contains("success") || content.Contains("check your email");

      return result
        ? FormSubmitResult.Successful()
        : FormSubmitResult.Failed(content);
    }
  }
}