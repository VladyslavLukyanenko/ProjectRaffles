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
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms;
using ProjectIndustries.ProjectRaffles.Core.Services.Templates;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.GoogleFormsModule
{
  [RaffleModuleName("Google Forms")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.GoogleForms)]
  public class GoogleForms : RaffleModuleBase, IDynamicFormsModule
  {
    private readonly IGoogleFormsClient _client;

    private IEnumerable<Field> _additionalFields = Array.Empty<Field>();
    private readonly IFormParser _parser;
    private GoogleFormParseResult _formParseResult;
    private readonly ICaptchaSolveService _captchaSolveService;

    private static readonly Regex ValidUrlRegex = new Regex(
      @"https:\/\/docs\.google\.com\/forms\/d\/e\/([^\/]*)/viewform",
      RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly IRaffleModuleExpandContextFactory _expandContextFactory;
    private readonly ITemplateExpandService _expandService;

    public GoogleForms(IEnumerable<IFormParser> parsers, IGoogleFormsClient client,
      IRaffleModuleExpandContextFactory expandContextFactory, ITemplateExpandService expandService,
      ICaptchaSolveService captchaSolveService)
    {
      _parser = parsers.First(_ => _.IsModuleSupported(RaffleModuleType.GoogleForms));
      _client = client;
      _expandContextFactory = expandContextFactory;
      _expandService = expandService;
      _captchaSolveService = captchaSolveService;
    }

    public override IEnumerable<Field> AdditionalFields => _additionalFields;

    protected override async Task ExecuteAsync(Profile profile, CancellationToken ct)
    {
      var payload = _additionalFields.Where(_ => !_.IsEmpty)
        .Select(f => new KeyValuePair<string, string>(f.SystemName, f.Value?.ToString()))
        .ToList();

      var hostnameBuilder = new UriBuilder(_formParseResult.SourceUrl)
      {
        Query = ""
      };
      var sourceUrl = hostnameBuilder.Uri.ToString();
      if (_formParseResult.RequiresSolveCaptcha)
      {
        Status = RaffleStatus.SolvingCAPTCHA;

        var solveResult = await _captchaSolveService.SolveReCaptchaV2Async(_formParseResult.SiteCaptchaKey,
          sourceUrl, true, ct);
        payload.Add(new KeyValuePair<string, string>("g-recaptcha-response", solveResult));
      }


      Status = RaffleStatus.Submitting;

      Status = await _client.SubmitAsync(payload, sourceUrl, ct)
        ? RaffleStatus.Succeeded
        : RaffleStatus.Failed;
    }

    protected override void SetFields(IEnumerable<Field> fields)
    {
      _additionalFields = fields.Select(_ => _.Clone()).ToList();
    }

    public override async Task InitializeAsync(IRaffleExecutionContext context, CancellationToken ct)
    {
      await base.InitializeAsync(context, ct);
      var expandContext = _expandContextFactory.Create(this, context);
      foreach (var strField in AdditionalFields.OfType<Field<string>>())
      {
        if (string.IsNullOrWhiteSpace(strField.Value))
        {
          continue;
        }

        strField.Value = _expandService.Expand(strField, expandContext);
      }
    }

    public override void InitializeFromPrototype(IRaffleModule module)
    {
      base.InitializeFromPrototype(module);
      var googleForms = ((GoogleForms) module);
      _formParseResult = googleForms._formParseResult;
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      return Task.FromResult(new Product {Name = _formParseResult.Title ?? "<Unnamed Google Form>"});
    }

    protected override IModuleHttpClient HttpClient => _client;

    public async ValueTask<FormParseResult> FetchFieldsAsync(Uri formUrl, CancellationToken ct = default)
    {
      _formParseResult = (GoogleFormParseResult) await _parser.ParseAsync(formUrl, ct);
      _additionalFields = _formParseResult.Fields;
      await OnAdditionalFieldsChanged(ct);

      return _formParseResult;
    }

    public bool IsUrlValid(string formUrl) => !string.IsNullOrEmpty(formUrl) && ValidUrlRegex.IsMatch(formUrl);
  }
}