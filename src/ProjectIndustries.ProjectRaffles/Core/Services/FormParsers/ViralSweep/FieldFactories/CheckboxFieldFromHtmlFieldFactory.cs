using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.ViralSweep.FieldFactories
{
  public class CheckboxFieldFromHtmlFieldFactory : IFieldFromHtmlFieldFactory
  {
    public bool CanHandle(IEnumerable<string> classes) => classes.Contains("checkbox");

    public IEnumerable<Field> Create(IHtmlElement element)
    {
      return element.QuerySelectorAll("input")
        .OfType<IHtmlInputElement>()
        .Select(i =>
        {
          var lbl = element.QuerySelector($"label[for='{i.Id}']").TextContent.Trim();
          return new CheckboxField<string>(i.Value, i.Name, lbl);
        })
        .ToArray();
    }
  }
}