// #define USE_JSON

#define DONT_USE_JSON
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Html
{
  public class MailChimpHtmlFormParser : IMailChimpHtmlFormParser
  {
    private const string FieldsSelector = "input,select,textarea";
    private readonly IMailchimpService _mailchimpService;
    private readonly ICaptchaSolveService _captchaSolveService;

    public MailChimpHtmlFormParser(IMailchimpService mailchimpService, ICaptchaSolveService captchaSolveService)
    {
      _mailchimpService = mailchimpService;
      _captchaSolveService = captchaSolveService;
    }

    public MailChimpParseResult Parse(IHtmlFormElement form, CookieContainer cookieContainer)
    {
      var submitUrl = form.Action;
      var createdFieldNames = new HashSet<string>();
      var createdFields = new List<Field>();

      var fields = form.QuerySelectorAll(FieldsSelector);
      foreach (var fieldGroup in fields.GroupBy(f => f.Attributes["name"]?.Value.SanitizeQuerySelector()))
      {
        foreach (var p in CreateAllFormFields(fieldGroup, form))
        {
          if (createdFieldNames.Contains(p.SystemName))
          {
            continue;
          }

          createdFieldNames.Add(p.SystemName);
          createdFields.Add(p);
        }
      }

      return new MailChimpParseResult(createdFields,
        new MixedMailChimpHtmlFormSubmitHandler(_mailchimpService, cookieContainer, new Uri(submitUrl),
          _captchaSolveService));
    }


    private IEnumerable<Field> CreateAllFormFields(IGrouping<string, IElement> fieldGroup, IHtmlFormElement form)
    {
      var group = fieldGroup.ToArray();
      var id = group[0].Attributes["id"]?.Value.SanitizeQuerySelector();
      IElement label = null;
      if (!string.IsNullOrEmpty(id))
      {
        label = form.QuerySelector($"[for={id}]");
      }

      var element = group[0];
      if (element.GetInputName()?.StartsWith("b_") == true && element.Attributes["tabindex"].Value == "-1")
      {
        yield return new HiddenField(element.GetInputName(), element.GetInputValue());
      }

      var ul = element.GetParent(_ => _.TagName.ToLowerInvariant() == "ul" || _.ClassList.Contains("radio-group"));
      if (ul != null)
      {
        if (ul.ClassList.Contains("checkbox-group"))
        {
          foreach (var checkbox in CreateCheckboxes(ul.ParentElement))
          {
            yield return checkbox;
          }
        }
        else
        {
          yield return CreateRadioButton(ul.ClassList.Contains("radio-group") ? ul.ParentElement : ul);
        }

        yield break;
      }

      var datefieldContainer = element.GetParent(e => e.ClassList.Contains("subfield"));
      if (datefieldContainer != null)
      {
        yield return CreateSubfield(datefieldContainer.ParentElement, form, element);
        yield break;
      }

      var fwField = Create(group, label);
      if (fwField == null)
      {
        yield break;
      }

      yield return fwField;
    }

    private static Field CreateSubfield(IElement datefieldContainer, IHtmlFormElement form, IElement element)
    {
      var firstSubfield = datefieldContainer.QuerySelector("input");
      var subfieldLabel = form.QuerySelector($"[for={firstSubfield.Id.SanitizeQuerySelector()}]");

      var subfieldDisplayName = subfieldLabel?.TextContent.Trim();
      var placeholder = element.Attributes["placeholder"]?.Value;
      if (!string.IsNullOrEmpty(placeholder)
          && !Equals(subfieldDisplayName?.ToLowerInvariant(), placeholder?.ToLowerInvariant()))
      {
        subfieldDisplayName += " " + placeholder;
      }

      return CreateField(element, element.GetInputValue(), element.GetInputName(), subfieldDisplayName,
        element.IsRequired());
    }

    private static Field CreateRadioButton(IElement fieldGroup)
    {
      var rootLabel = fieldGroup.ParentElement?.FirstElementChild;
      var rootLabelText = rootLabel?.TextContent.Trim();
      var element = fieldGroup.QuerySelector("input");
      var systemName = element.GetInputName();
      var isRequired = element.IsRequired();

      var options = fieldGroup.QuerySelectorAll("input")
        .Select(e => e.GetInputValue())
        .ToArray();

      var labelText = fieldGroup.FirstElementChild.TextContent.Trim();

      return new OptionsField(systemName, rootLabelText ?? labelText, isRequired, options);
    }

    private static IEnumerable<Field> CreateCheckboxes(IElement fieldGroup)
    {
      var rootLabel = fieldGroup.ParentElement?.FirstElementChild;
      var rootLabelText = rootLabel?.TextContent.Trim();

      foreach (var checkbox in fieldGroup.QuerySelectorAll("input"))
      {
        var parentLi = checkbox.GetParent("li");
        var label = parentLi.FindLabelFor(checkbox.Id);
        var ownLabel = label?.TextContent.Trim();
        if (!string.IsNullOrEmpty(rootLabelText))
        {
          ownLabel = rootLabelText + " - " + ownLabel;
        }

        yield return new CheckboxField<string>(checkbox.GetInputValue(), checkbox.GetInputName(), ownLabel);
      }
    }

    private static Field CreateDynamicPickerField(string systemName, string labelText, bool isRequired)
    {
      var startIdx = systemName.IndexOf('[') + 1;
      var addressPartType = systemName[startIdx..^1];
      return new DynamicValuesPickerField(systemName, labelText, isRequired, groups: Pickers.All)
      {
        SelectedResolver = MailChimpFieldValueResolverFactory.GetForAddressPart(addressPartType)
                           ?? MailChimpFieldValueResolverFactory.GetForRegularField(systemName.ToLowerInvariant())
      };
    }


    private Field Create(IEnumerable<IElement> group, IElement label)
    {
      var labelText = label?.TextContent.Trim();
      var element = group.First();

      var systemName = element.GetInputName();
      var isRequired = element.IsRequired();
      labelText ??= systemName;
      var value = element.GetInputValue();
      if (label == null)
      {
        return new HiddenField(systemName, value);
      }

      return CreateField(element, value, systemName, labelText, isRequired);
    }

    private static Field CreateField(IElement element, string value, string systemName, string labelText,
      bool isRequired)
    {
      return element.TagName.ToLowerInvariant() switch
      {
        "input" => element.Attributes["type"].Value switch
        {
          "checkbox" => new CheckboxField<string>(value, systemName, labelText, isRequired),
          "hidden" => new HiddenField(systemName, value),
          "submit" => null,
          _ => CreateDynamicPickerField(systemName, labelText, isRequired),
        },
        "textarea" => new TemplatedMultilineField(systemName, labelText, isRequired),
        "select" => CreateOptionsField(element, systemName, labelText, isRequired),
        _ => throw new InvalidOperationException("Not supported tag name " + element.TagName)
      };
    }

    private static Field CreateOptionsField(IElement input, string systemName, string labelText, bool isRequired)
    {
      var options = input.QuerySelectorAll("option")
        .Select(e => new KeyValuePair<string, string>(e.TextContent.Trim(), e.Attributes["value"].Value))
        .ToArray();

      return new OptionsField(systemName, labelText, isRequired, options);
    }
  }
}