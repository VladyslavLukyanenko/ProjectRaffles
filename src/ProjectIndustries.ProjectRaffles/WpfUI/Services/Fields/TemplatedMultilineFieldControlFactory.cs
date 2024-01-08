using System.Windows.Controls;
using System.Windows.Data;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using Xceed.Wpf.Toolkit;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services.Fields
{
  public class TemplatedMultilineFieldControlFactory : SingleFieldControlFactoryBase<TemplatedMultilineField>
  {
    protected override Control CreateEditorControl(TemplatedMultilineField field)
    {
      var control = new WatermarkTextBox
      {
        Watermark = field.DisplayName,
        AcceptsReturn = true,
        AcceptsTab = true
      };

      control.MinHeight = 100;

      var binding = new Binding(nameof(Field.Value))
      {
        Source = field,
        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
      };

      control.SetBinding(TextBox.TextProperty, binding);

      return control;
    }
  }
}