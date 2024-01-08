using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.ViralSweep.FieldFactories
{
  public class PhoneFieldFromHtmlFieldFactory : IFieldFromHtmlFieldFactory
  {
    public bool CanHandle(IEnumerable<string> classes) => classes.Contains("phone");

    public IEnumerable<Field> Create(IHtmlElement element)
    {
      var inputs = element.QuerySelectorAll("input")
        .OfType<IHtmlInputElement>()
        .ToArray();

      var codeHolder = inputs.FirstOrDefault(_ => _.Type == "hidden");
      // var htmlListItems = element.QuerySelectorAll(".country-list li");
      // var listItems = htmlListItems
      //   .OfType<IHtmlListItemElement>()
      //   .Where(_ => _.ClassList.Contains("country"))
      //   .Select(_ => new KeyValuePair<string, object>(Uri.UnescapeDataString(_.TextContent).Trim(),
      //     _.Attributes["data-dial-code"].Value))
      //   .ToArray();
      //
      // yield return new SelectField(, , true, options: listItems)
      // {
      //   Value = codeHolder.GetInputValue()
      // };

      var i = inputs.First(_ => _.Type != "hidden");
      bool isRequired = i.ClassList.Contains("is_required") || i.HasAttribute("required");
      if (codeHolder != null)
      {
        yield return new TextField(codeHolder.GetInputName(), "Phone Code", isRequired);
      }

      yield return new TextField(i.GetInputName(), i.Attributes["placeholder"]?.Value, isRequired);
      /*yield return new DynamicValuesPickerField(i.GetInputName(), i.Attributes["placeholder"]?.Value,
      i.HasAttribute("required"))
    {
      SelectedResolver = Pickers.ProfileShippingAddressFields.PhoneNumber
    };*/
    }
  }
}