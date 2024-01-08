using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.ViralSweep.FieldFactories
{
  public class TextFieldFromHtmlFieldFactory : IFieldFromHtmlFieldFactory
  {
    private static readonly string[] SupportedTypes =
    {
      "name", "email", "address", "address2", "city", "zip", "text", "textarea", "code"
    };

    public bool CanHandle(IEnumerable<string> classes) => classes.Any(c => SupportedTypes.Contains(c));

    public IEnumerable<Field> Create(IHtmlElement element)
    {
      return element.QuerySelectorAll("input")
        .Select(i =>
          new DynamicValuesPickerField(i.GetInputName(), i.Attributes["placeholder"]?.Value, i.HasAttribute("required"),
            groups: Pickers.All)
          {
            SelectedResolver = ViralSweepFieldValueResolverFactory.Resolve(element.ClassList, i.GetInputName())
          });
    }
  }
}