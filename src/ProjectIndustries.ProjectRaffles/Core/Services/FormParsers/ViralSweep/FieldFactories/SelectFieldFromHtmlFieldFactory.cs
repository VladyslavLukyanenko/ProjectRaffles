using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.ViralSweep.FieldFactories
{
  public class SelectFieldFromHtmlFieldFactory : IFieldFromHtmlFieldFactory
  {
    private static readonly string[] SupportedTypes =
    {
      "state", "country", "dropdown"
    };

    public bool CanHandle(IEnumerable<string> classes) => classes.Any(c => SupportedTypes.Contains(c));

    public IEnumerable<Field> Create(IHtmlElement element)
    {
      var select = (IHtmlSelectElement) element.QuerySelector("select");
      var options = select.GetOptionsWithStrValue();

      var displayLabel = select.Options.FirstOrDefault(_ => _.HasAttribute("invalid"));
      yield return new OptionsField(select.Name, displayLabel?.Text, element.HasAttribute("required"), options);
    }
  }
}