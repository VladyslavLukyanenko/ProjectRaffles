using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services.Fields
{
  public class SelectFieldControlFactory : SingleFieldControlFactoryBase<SelectField>
  {
    protected override Control CreateEditorControl(SelectField field)
    {
      var itemsSource = new Binding("")
      {
        Source = field.Options,
      };
      
      var selectedValue = new Binding(nameof(Field.Value))
      {
        Source = field
      };
      
      var control = new ComboBox
      {
        DisplayMemberPath = nameof(KeyValuePair<string, object>.Key),
        SelectedValuePath = nameof(KeyValuePair<string, object>.Value),
        Style = (Style) Application.Current.FindResource("StandardComboBox")
      };

      control.SetBinding(ItemsControl.ItemsSourceProperty, itemsSource);
      control.SetBinding(Selector.SelectedValueProperty, selectedValue);

      return control;
    }
  }
}