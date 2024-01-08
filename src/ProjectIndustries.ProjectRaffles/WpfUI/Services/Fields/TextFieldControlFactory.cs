using System.Windows.Controls;
using System.Windows.Data;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using Xceed.Wpf.Toolkit;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services.Fields
{
  public class TextFieldControlFactory : SingleFieldControlFactoryBase<TextField>
  {
    protected override Control CreateEditorControl(TextField field)
    {
      var control = new WatermarkTextBox
      {
        Watermark = field.DisplayName
      };

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