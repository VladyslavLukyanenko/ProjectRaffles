using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using DynamicData;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Services.Accounts;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services.Fields
{
  public class AccountPickerFieldControlFactory : SingleFieldControlFactoryBase<AccountPickerField>
  {
    private readonly ReadOnlyObservableCollection<AccountGroup> _accountGroups;

    public AccountPickerFieldControlFactory(IAccountGroupsRepository accountGroupsRepository)
    {
      accountGroupsRepository.Items.Connect()
        .Bind(out _accountGroups)
        .DisposeMany()
        .Subscribe();
    }

    protected override Control CreateEditorControl(AccountPickerField field)
    {
      var itemsSource = new Binding("")
      {
        Source = _accountGroups,
      };

      var selectedValue = new Binding(nameof(Field.Value))
      {
        Source = field
      };

      var control = new ComboBox
      {
        DisplayMemberPath = nameof(AccountGroup.Name)
      };

      control.SetBinding(ItemsControl.ItemsSourceProperty, itemsSource);
      control.SetBinding(Selector.SelectedItemProperty, selectedValue);

      return control;
    }
  }
}