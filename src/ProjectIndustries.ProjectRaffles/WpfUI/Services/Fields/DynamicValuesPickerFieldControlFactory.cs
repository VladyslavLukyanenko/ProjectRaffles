using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services.Fields
{
  public class DynamicValuesPickerFieldControlFactory : SingleFieldControlFactoryBase<DynamicValuesPickerField>
  {
    protected override Control CreateEditorControl(DynamicValuesPickerField field)
    {
      var options = field.Groups.SelectMany(g =>
          g.ValueResolvers.Select(r => new GroupedResolver(g.Group, r)))
        .ToList();

      var lcv = new ListCollectionView(options);

      lcv.GroupDescriptions!.Add(new PropertyGroupDescription(nameof(GroupedResolver.GroupName)));


      var headerTemplate = new DataTemplate(typeof(GroupedResolver));
      var factory = new FrameworkElementFactory(typeof(TextBlock));
      factory.SetBinding(TextBlock.TextProperty, new Binding("Name"));
      factory.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromRgb(60, 60, 60)));
      factory.SetValue(FrameworkElement.MarginProperty, new Thickness(11, 8, 0, 0));

      headerTemplate.VisualTree = factory;

      var control = new ComboBox
      {
        GroupStyle =
        {
          new GroupStyle
          {
            HeaderTemplate = headerTemplate
          }
        },
        DisplayMemberPath = nameof(GroupedResolver.ResolverName),
        SelectedValuePath = nameof(GroupedResolver.Resolver)
      };

      var itemsSource = new Binding("")
      {
        Source = lcv
      };
      control.SetBinding(ItemsControl.ItemsSourceProperty, itemsSource);

      var selectedValue = new Binding(nameof(GroupedResolver.Resolver))
      {
        Source = field,
        Path = new PropertyPath(nameof(DynamicValuesPickerField.SelectedResolver))
      };
      control.SetBinding(Selector.SelectedValueProperty, selectedValue);
      lcv.MoveCurrentTo(options.FirstOrDefault(_ => _.Resolver == field.SelectedResolver));

      return control;
    }

    private class GroupedResolver
    {
      public GroupedResolver(string groupName, IDynamicValueResolver resolver)
      {
        GroupName = groupName;
        Resolver = resolver;
      }

      public string GroupName { get; private set; }
      public string ResolverName => Resolver.Name;
      public IDynamicValueResolver Resolver { get; private set; }
    }
  }
}