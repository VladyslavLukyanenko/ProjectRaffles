using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services.Fields
{
  public class CheckboxFieldControlFactory : SingleFieldControlFactoryBase<CheckboxField>
  {
    public override UIElement Create(Field field)
    {
      var panel = new StackPanel();
      var label = new TextBlock
      {
        Text = " "
      };

      panel.Children.Add(label);

      var control = CreateEditorControl((CheckboxField) field);
      panel.Children.Add(control);

      return panel;
    }

    protected override Control CreateEditorControl(CheckboxField field)
    {
      var control = new CheckBox
      {
        Content = field.DisplayName
      };
      
      var binding = new Binding(nameof(CheckboxField.IsChecked))
      {
        Source = field,
      };

      control.SetBinding(ToggleButton.IsCheckedProperty, binding);

      return control;
    }
  }
}