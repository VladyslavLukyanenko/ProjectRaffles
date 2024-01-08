using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using DynamicData;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Services.Proxies;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services.Fields
{
  public class ProxyPickerFieldControlFactory : SingleFieldControlFactoryBase<ProxyGroupPickerField>
  {
    private readonly ReadOnlyObservableCollection<ProxyGroup> _proxyGroups;

    public ProxyPickerFieldControlFactory(IProxyGroupsRepository proxyGroupsRepository)
    {
      proxyGroupsRepository.Items.Connect()
        .Bind(out _proxyGroups)
        .DisposeMany()
        .Subscribe();
    }

    protected override Control CreateEditorControl(ProxyGroupPickerField field)
    {
      var itemsSource = new Binding("")
      {
        Source = _proxyGroups,
      };

      var selectedValue = new Binding(nameof(Field.Value))
      {
        Source = field
      };

      var control = new ComboBox
      {
        DisplayMemberPath = nameof(ProxyGroup.Name)
      };

      control.SetBinding(ItemsControl.ItemsSourceProperty, itemsSource);
      control.SetBinding(Selector.SelectedItemProperty, selectedValue);

      return control;
    }
  }
}