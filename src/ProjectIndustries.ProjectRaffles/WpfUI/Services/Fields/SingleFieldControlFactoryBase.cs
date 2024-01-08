using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services.Fields
{
  public abstract class SingleFieldControlFactoryBase<T> : IFieldControlFactory
    where T : Field
  {
    public bool IsSupported(Field field) => field is T;

    public virtual UIElement Create(Field field)
    {
      var panel = new StackPanel();
      var label = new TextBlock
      {
        Text = field.DisplayName + (field.IsRequired ? " *" : ""),
        Foreground = new SolidColorBrush(Color.FromRgb(106, 106, 106))
      };

      panel.Children.Add(label);

      var control = CreateEditorControl((T) field);
      panel.Children.Add(control);

      return panel;
    }

    protected abstract Control CreateEditorControl(T field);
  }
}