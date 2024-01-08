using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services.Fields
{
  public class RadioButtonFieldControlFactory : IFieldControlFactory
  {
    public bool IsSupported(Field field) => field is RadioButtonGroupField;

    public UIElement Create(Field field)
    {
      var stackpanel = new StackPanel();
      var emptyLabel = new TextBlock
      {
        Text = field.DisplayName,
        Foreground = new SolidColorBrush(Color.FromRgb(106, 106, 106))
      };

      stackpanel.Children.Add(emptyLabel);
      
      var radioGroup = new StackPanel
      {
        Orientation = Orientation.Horizontal
      };

      stackpanel.Children.Add(radioGroup);

      var group = (RadioButtonGroupField) field;
      foreach (var pair in group.NameValues)
      {
        var control = new RadioButton
        {
          GroupName = field.SystemName,
          Content = pair.Key,
          Margin = new Thickness(0, 0, 5, 0)
        };

        control.WhenAnyValue(_ => _.IsChecked)
          .Where(isChecked => isChecked == true)
          .Subscribe(_ =>
          {
            field.Value = pair.Value;
          });

        radioGroup.Children.Add(control);
      }

      return stackpanel;
    }
  }
}