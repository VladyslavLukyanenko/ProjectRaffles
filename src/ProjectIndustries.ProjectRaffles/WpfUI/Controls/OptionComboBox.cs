using System.Windows;
using System.Windows.Controls;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Controls
{
  public class OptionComboBox : ComboBox
  {
    public static readonly DependencyProperty SelectRandomProperty = DependencyProperty.Register(nameof(SelectRandom),
      typeof(bool), typeof(OptionComboBox), new PropertyMetadata(OnSelectRandomChanged));

    public bool SelectRandom
    {
      get => (bool) GetValue(SelectRandomProperty);
      set => SetValue(SelectRandomProperty, value);
    }

    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
      base.OnSelectionChanged(e);
      if (SelectedItem == null)
      {
        return;
      }

      SelectRandom = false;
    }

    private static void OnSelectRandomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var selectRandom = (bool) e.NewValue;
      if (selectRandom)
      {
        d.SetValue(SelectedValueProperty, null);
      }
    }
  }
}