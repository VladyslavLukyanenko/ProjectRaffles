using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.ViralSweep.FieldFactories
{
  public class BirthdayFieldFromHtmlFieldFactory : IFieldFromHtmlFieldFactory
  {
    public bool CanHandle(IEnumerable<string> classes) => classes.Contains("birthday");

    public IEnumerable<Field> Create(IHtmlElement element)
    {
      return element.QuerySelectorAll("select")
        .OfType<IHtmlSelectElement>()
        .Select(s =>
        {
          var displayLabel = s.Options.First(_ => _.HasAttribute("invalid"));
          return new SelectField(s.Name, displayLabel.Text, s.HasAttribute("required"),
            options: s.GetOptionsAsObjValue());
        });
    }
  }
}