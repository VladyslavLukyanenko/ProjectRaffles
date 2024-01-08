using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.ViralSweep.FieldFactories
{
  public class RadioButtonsFieldFromHtmlFieldFactory : IFieldFromHtmlFieldFactory
  {
    public bool CanHandle(IEnumerable<string> classes) => classes.Contains("radio");

    public IEnumerable<Field> Create(IHtmlElement element)
    {
      var label = element.QuerySelector(".form_label_text")
        .TextContent
        .Trim();
      var systemName = element.Attributes["data-tep"].Value;
      var options = element.QuerySelectorAll("input")
        .OfType<IHtmlInputElement>()
        .Select(s =>
        {
          var displayLbl = element.QuerySelector($"label[for='{s.Id}']").TextContent.Trim();
          return new KeyValuePair<string, string>(displayLbl, s.Value);
        });

      yield return new OptionsField(systemName, label, false, options);
    }
  }
}