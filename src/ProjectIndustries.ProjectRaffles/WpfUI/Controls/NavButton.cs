using System.Windows;
using System.Windows.Controls;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Controls
{
  public class NavButton
    : Button
  {
    public static readonly DependencyProperty RegularIconSrcProperty =
      DependencyProperty.Register(nameof(RegularIconSrc), typeof(string), typeof(NavButton));

    public static readonly DependencyProperty ActiveIconSrcProperty =
      DependencyProperty.Register(nameof(ActiveIconSrc), typeof(string), typeof(NavButton));

    public static readonly DependencyProperty IsActiveProperty =
      DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(NavButton));


    public string RegularIconSrc
    {
      get => (string) GetValue(RegularIconSrcProperty);
      set => SetValue(RegularIconSrcProperty, value);
    }

    public string ActiveIconSrc
    {
      get => (string) GetValue(ActiveIconSrcProperty);
      set => SetValue(ActiveIconSrcProperty, value);
    }

    public bool IsActive
    {
      get => (bool) GetValue(IsActiveProperty);
      set => SetValue(IsActiveProperty, value);
    }
  }
}