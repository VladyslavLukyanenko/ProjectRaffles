using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Services.Templates;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Emails
{
  public class GeneralSettingsBasedCatchAllEmailMaterializer : ICatchAllEmailMaterializer
  {
    private readonly IGeneralSettingsService _generalSettingsService;
    private readonly ITemplateExpandService _expandService;
    private readonly ITemplateExpandContext _templateExpandContext;

    public GeneralSettingsBasedCatchAllEmailMaterializer(IGeneralSettingsService generalSettingsService,
      ITemplateExpandService expandService, ITemplateExpandContext templateExpandContext)
    {
      _generalSettingsService = generalSettingsService;
      _expandService = expandService;
      _templateExpandContext = templateExpandContext;
    }

    public string Materialize(string catchallEmail)
    {
      var tmpl = _generalSettingsService.CurrentSettings.CatchAllEmailMaterializeTemplate;
      if (string.IsNullOrWhiteSpace(tmpl))
      {
        return null;
      }

      var result = _expandService.Expand(tmpl, _templateExpandContext);

      return result + catchallEmail;
    }
  }
}