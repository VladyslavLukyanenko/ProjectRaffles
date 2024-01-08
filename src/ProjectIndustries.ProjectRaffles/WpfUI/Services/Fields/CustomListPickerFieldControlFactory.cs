using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using DynamicData;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.CustomLists;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services.Fields
{
  public class CustomListPickerFieldControlFactory : SingleFieldControlFactoryBase<CustomListPickerField>
  {
    private readonly ReadOnlyObservableCollection<CustomList> _lists;

    public CustomListPickerFieldControlFactory(ICustomListRepository repository)
    {
      repository.Items.Connect()
        .Bind(out _lists)
        .DisposeMany()
        .Subscribe();
    }

    protected override Control CreateEditorControl(CustomListPickerField field)
    {
      var itemsSource = new Binding("")
      {
        Source = _lists,
      };

      var selectedValue = new Binding(nameof(Field.Value))
      {
        Source = field
      };

      var control = new ComboBox
      {
        DisplayMemberPath = nameof(CustomList.Name)
      };

      control.SetBinding(ItemsControl.ItemsSourceProperty, itemsSource);
      control.SetBinding(Selector.SelectedItemProperty, selectedValue);

      return control;
    }
  }
}