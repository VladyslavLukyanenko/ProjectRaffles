using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.WpfUI.Controls;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services.Fields
{
  public class OptionsFieldControlFactory : SingleFieldControlFactoryBase<OptionsField>
  {
    protected override Control CreateEditorControl(OptionsField field)
    {
      var itemsSource = new Binding("")
      {
        Source = field.Options,
      };
      
      var selectedValue = new Binding(nameof(OptionsField.SelectedValue))
      {
        Source = field
      };
      
      var selectRandom = new Binding(nameof(OptionsField.PickRandom))
      {
        Source = field,
        Mode = BindingMode.TwoWay
      };
      
      var control = new OptionComboBox
      {
        DisplayMemberPath = nameof(KeyValuePair<string, string>.Key),
        SelectedValuePath = nameof(KeyValuePair<string, string>.Value),
      };

      control.SetBinding(ItemsControl.ItemsSourceProperty, itemsSource);
      control.SetBinding(Selector.SelectedValueProperty, selectedValue);
      control.SetBinding(OptionComboBox.SelectRandomProperty, selectRandom);

      return control;
    }
  }
}