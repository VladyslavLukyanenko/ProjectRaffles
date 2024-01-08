using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.ViralSweep.FieldFactories
{
  public class PaymentCardFieldsFromHtmlFieldFactory : IFieldFromHtmlFieldFactory
  {
    public bool CanHandle(IEnumerable<string> classes) => classes.Contains("stripe_payment_input_form");

    public IEnumerable<Field> Create(IHtmlElement element)
    {
      return element.QuerySelectorAll("input")
        .OfType<IHtmlInputElement>()
        .Select(i =>
        {
          var lbl = element.QuerySelector($"label[for='{i.Id}']").TextContent.Trim();
          return new DynamicValuesPickerField(i.Name, lbl, i.HasAttribute("required"), groups: Pickers.All)
          {
            SelectedResolver = ViralSweepFieldValueResolverFactory.Resolve(i.ClassList, i.Id)
          };
        })
        .ToArray();
    }
  }
}