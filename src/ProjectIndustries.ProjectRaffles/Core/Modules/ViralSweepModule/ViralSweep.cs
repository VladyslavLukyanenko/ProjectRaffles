using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.ViralSweep;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.ViralSweepModule
{
  [RaffleModuleName("ViralSweep")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.Viralsweep)]
  public class ViralSweep : RaffleModuleBase, IDynamicFormsModule
  {
    private IEnumerable<Field> _additionalFields = Array.Empty<Field>();
    private readonly IFormParser _parser;
    private ViralSweepParseResult _formParseResult;
    private readonly ICaptchaSolveService _captchaSolveService;

    private static readonly Regex ValidUrlRegex = new Regex(@".*", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public ViralSweep(IEnumerable<IFormParser> parsers, ICaptchaSolveService captchaSolveService)
    {
      _captchaSolveService = captchaSolveService;
      _parser = parsers.First(_ => _.IsModuleSupported(RaffleModuleType.Viralsweep));
    }

    public override IEnumerable<Field> AdditionalFields => _additionalFields;

    protected override async Task ExecuteAsync(Profile profile, CancellationToken ct)
    {
      var fields = AdditionalFields.ToList();
      if (!string.IsNullOrEmpty(_formParseResult.ReCaptchaSiteKey))
      {
        Status = RaffleStatus.SolvingCAPTCHA;
        var token = await _captchaSolveService.SolveReCaptchaV2Async(_formParseResult.ReCaptchaSiteKey,
          _formParseResult.FormUrl, false, ct);
        fields.Add(new HiddenField("g-recaptcha-response", token));
      }

      var result = await _formParseResult.SubmitHandler.SubmitAsync(_formParseResult.SourceUrl, fields, ct);
      Status = result.IsSuccess
        ? RaffleStatus.Succeeded
        : RaffleStatus.FailedWithCause("Failed to submit form", result.ErrorMessage);
    }

    protected override void SetFields(IEnumerable<Field> fields)
    {
      _additionalFields = fields.Select(_ => _.Clone()).ToList();
    }

    public override void InitializeFromPrototype(IRaffleModule module)
    {
      base.InitializeFromPrototype(module);
      var forms = (ViralSweep) module;
      _formParseResult = forms._formParseResult;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      return Task.FromResult(new Product {Name = _formParseResult.Title ?? "<Unnamed ViralSweep Form>"});
    }

    public override void SetHttpClientBuilder(IHttpClientBuilder builder)
    {
    }

    protected override IModuleHttpClient HttpClient => null;

    public async ValueTask<FormParseResult> FetchFieldsAsync(Uri formUrl, CancellationToken ct = default)
    {
      _formParseResult = (ViralSweepParseResult) await _parser.ParseAsync(formUrl, ct);
      _additionalFields = _formParseResult.Fields;
      await OnAdditionalFieldsChanged(ct);

      return _formParseResult;
    }

    public bool IsUrlValid(string formUrl) => !string.IsNullOrEmpty(formUrl) && ValidUrlRegex.IsMatch(formUrl);
  }
}