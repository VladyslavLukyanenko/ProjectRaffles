using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TypeFormsModule
{
  [RaffleModuleName("Type Forms")]
  [RaffleModuleVersion(0, 0, 1)]
  [RaffleModuleType(RaffleModuleType.TypeForms)]
  public class TypeForms : RaffleModuleBase, IDynamicFormsModule
  {
    private readonly ITypeFormsClient _client;

    private IEnumerable<Field> _additionalFields = Array.Empty<Field>();
    private readonly IFormParser _parser;
    private TypeFormParseResult _formParseResult;

    private static readonly Regex ValidUrlRegex = new Regex(@"https:\/\/.*\.typeform\.com\/to\/.*",
      RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public TypeForms(IEnumerable<IFormParser> parsers, ITypeFormsClient client)
    {
      _parser = parsers.First(_ => _.IsModuleSupported(RaffleModuleType.TypeForms));
      _client = client;
    }

    public override IEnumerable<Field> AdditionalFields => _additionalFields;

    protected override async Task ExecuteAsync(Profile profile, CancellationToken ct)
    {
      // NOTICE:
      // we need to copy values from current AdditionalFields to typeform fields
      // because THEY will be used to construct submit payload
      var targetFields = _formParseResult.TypeFormFields.SelectMany(_ => _.Fields).ToArray();
      foreach (var field in AdditionalFields)
      {
        var target = targetFields.First(_ => _.SystemName == field.SystemName);
        field.CopyTo(target);
      }

      var sourceUrl = _formParseResult.SourceUrl.ToString();
      Status = RaffleStatus.GettingRaffleInfo;
      var submissionDetails = await _client.StartSubmissionAsync(sourceUrl, ct);
      
      Status = RaffleStatus.Waiting;
      var rnd = new Random();
      var wait = rnd.Next(20000, 60000); // generate ms to wait, either 20sec or 60sec
      
      await Task.Delay(wait, ct); //wait

      Status = RaffleStatus.Submitting;
      Status = await _client.SubmitAsync(sourceUrl,
        new TypeFormsSubmitPayload(submissionDetails, _formParseResult.TypeFormFields), ct) //todo: create
        ? RaffleStatus.Succeeded
        : RaffleStatus.Failed;
    }

    protected override void SetFields(IEnumerable<Field> fields)
    {
      _additionalFields = fields.Select(_ => _.Clone()).ToList();
    }

    public override void InitializeFromPrototype(IRaffleModule module)
    {
      base.InitializeFromPrototype(module);
      var forms = (TypeForms) module;
      _formParseResult = forms._formParseResult.Clone();
    }

    protected override Task<Product> FetchProductAsync(CancellationToken ct)
    {
      return Task.FromResult(new Product {Name = _formParseResult.Title ?? "<Unnamed Type Form>"});
    }

    protected override IModuleHttpClient HttpClient => _client;

    public async ValueTask<FormParseResult> FetchFieldsAsync(Uri formUrl, CancellationToken ct = default)
    {
      _formParseResult = (TypeFormParseResult) await _parser.ParseAsync(formUrl, ct);
      _additionalFields = _formParseResult.Fields;
      await OnAdditionalFieldsChanged(ct);

      return _formParseResult;
    }

    public bool IsUrlValid(string formUrl) => !string.IsNullOrEmpty(formUrl) && ValidUrlRegex.IsMatch(formUrl);
  }
}