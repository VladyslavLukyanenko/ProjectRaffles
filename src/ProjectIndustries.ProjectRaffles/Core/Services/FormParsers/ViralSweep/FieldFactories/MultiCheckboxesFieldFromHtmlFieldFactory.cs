using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AngleSharp.Html.Dom;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.ViralSweep.FieldFactories
{
  public class MultiCheckboxesFieldFromHtmlFieldFactory : IFieldFromHtmlFieldFactory
  {
    public bool CanHandle(IEnumerable<string> classes) => classes.Contains("multi_check");

    public IEnumerable<Field> Create(IHtmlElement element)
    {
      var label = element.QuerySelector(".form_label_text")
        .TextContent
        .Trim();

      var checkboxes = element.QuerySelectorAll("input[type=checkbox]")
        .OfType<IHtmlInputElement>()
        .Select(s =>
        {
          var lbl = element.QuerySelector($"label[for='{s.Id}']").TextContent.Trim();
          return new ViralSweepMultiCheckboxField(s.Name, $"{lbl} ({label})", s.Value);
        })
        .ToArray();
      foreach (var checkboxField in checkboxes)
      {
        yield return checkboxField;
      }

      var hidden = (IHtmlInputElement) element.QuerySelector("input[type=hidden]");
      yield return new ComposedCheckboxesField(hidden.Name, checkboxes);
    }

    private class ComposedCheckboxesField : HiddenField
    {
      public ComposedCheckboxesField(string systemName, IEnumerable<ViralSweepMultiCheckboxField> checkboxes)
        : base(systemName, systemName)
      {
        checkboxes.Select(_ => _.Changed)
          .CombineLatest()
          .Select(fields => fields.OfType<ViralSweepMultiCheckboxField>()
            .Where(_ => _.IsChecked)
            .Select(_ => _.Value))
          .Select(values => string.Join(",", values))
          .Subscribe(v => Value = v);
      }
    }
  }
  
  

  public class ViralSweepMultiCheckboxField : CheckboxField<string>
  {
    public ViralSweepMultiCheckboxField()
    {
    }

    public ViralSweepMultiCheckboxField(string value, string systemName = null, string displayName = null)
      : base(systemName, displayName, value)
    {
    }
  }
}