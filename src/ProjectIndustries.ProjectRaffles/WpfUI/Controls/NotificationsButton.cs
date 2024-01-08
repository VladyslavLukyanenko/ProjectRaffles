using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Controls
{
  public class NotificationsButton : ComboBox
  {
    public static readonly DependencyPropertyKey IsEmptyPropertyKey = DependencyProperty.RegisterReadOnly(
      nameof(IsEmpty), typeof(bool), typeof(NotificationsButton), new PropertyMetadata(true));

    public static readonly DependencyProperty IsEmptyProperty = IsEmptyPropertyKey.DependencyProperty;
    public static readonly DependencyProperty ClearCommandProperty = DependencyProperty.Register(nameof(ClearCommand),
      typeof(ICommand), typeof(NotificationsButton));

    static NotificationsButton()
    {
      // ItemsSourceProperty.OverrideMetadata(typeof(NotificationsButton), new PropertyMetadata(OnItemsSourceChanged))
    }


    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
      base.OnItemsChanged(e);
      SetValue(IsEmptyPropertyKey, Items.IsEmpty);
    }

    public bool IsEmpty => (bool) GetValue(IsEmptyProperty);

    public ICommand ClearCommand
    {
      get => (ICommand) GetValue(ClearCommandProperty);
      set => SetValue(ClearCommandProperty, value);
    }
  }
}