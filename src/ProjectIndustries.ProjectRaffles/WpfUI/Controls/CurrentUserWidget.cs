using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Controls
{
  public class CurrentUserWidget : ComboBox
  {
    static CurrentUserWidget()
    {
      //EventManager.RegisterClassHandler(typeof(CurrentUserWidget), Mouse.LostMouseCaptureEvent,
      //  new MouseEventHandler(OnLostMouseCapture));
      //EventManager.RegisterClassHandler(typeof(CurrentUserWidget), Mouse.MouseDownEvent,
      //  new MouseEventHandler(OnMouseDownEvent), true);
      EventManager.RegisterClassHandler(typeof(CurrentUserWidget), Mouse.PreviewMouseDownEvent,
        new MouseButtonEventHandler(OnPreviewMouseButtonDown));
    }

    private static void OnPreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
    {
      //CurrentUserWidget widget = (CurrentUserWidget)sender;
      //Visual originalSource = e.OriginalSource as Visual;
      //if (originalSource == null)
      //{
      //  return;
      //}
      //if (widget.IsDropDownOpen == true)
      //{
      //  widget.Close();
      //}
    }

    // private static void OnMouseDownEvent(object sender, MouseEventArgs e)
    // {
    //   CurrentUserWidget widget = (CurrentUserWidget) sender;
    //   if (!widget.IsKeyboardFocusWithin)
    //     widget.Focus();
    //   e.Handled = true;
    //   if (Mouse.Captured != widget || e.OriginalSource != widget)
    //   {
    //     return;
    //   }
    //
    //   widget.Close();
    // }
    //
    // private static void OnLostMouseCapture(object sender, MouseEventArgs e)
    // {
    //   CurrentUserWidget widget = (CurrentUserWidget) sender;
    //   if (Mouse.Captured == widget)
    //   {
    //     return;
    //   }
    //
    //   if (e.OriginalSource == widget)
    //   {
    //     // if (Mouse.Captured != null && MenuBase.IsDescendant((DependencyObject) widget, Mouse.Captured as DependencyObject))
    //     //   return;
    //     widget.Close();
    //   }
    //   // else if (MenuBase.IsDescendant((DependencyObject) comboBox, e.OriginalSource as DependencyObject))
    //   // {
    //   //   if (!comboBox.IsDropDownOpen || Mouse.Captured != null || !(SafeNativeMethods.GetCapture() == IntPtr.Zero))
    //   //     return;
    //   //   Mouse.Capture((IInputElement) comboBox, CaptureMode.SubTree);
    //   //   e.Handled = true;
    //   // }
    //   else
    //     widget.Close();
    // }
    //

    public static readonly DependencyProperty PictureProperty =
      DependencyProperty.Register(nameof(Picture), typeof(ImageSource), typeof(CurrentUserWidget));

    public ImageSource Picture
    {
      get => (ImageSource) GetValue(PictureProperty);
      set => SetValue(PictureProperty, value);
    }

    public static readonly DependencyProperty DeactivateCommandProperty =
      DependencyProperty.Register(nameof(DeactivateCommand), typeof(ICommand), typeof(CurrentUserWidget));

    public ICommand DeactivateCommand
    {
      get => (ICommand) GetValue(DeactivateCommandProperty);
      set => SetValue(DeactivateCommandProperty, value);
    }

    public static readonly DependencyProperty LogoutCommandProperty =
      DependencyProperty.Register(nameof(LogoutCommand), typeof(ICommand), typeof(CurrentUserWidget));

    public ICommand LogoutCommand
    {
      get => (ICommand) GetValue(LogoutCommandProperty);
      set => SetValue(LogoutCommandProperty, value);
    }

    public static readonly DependencyProperty UsernameProperty =
      DependencyProperty.Register(nameof(Username), typeof(string), typeof(CurrentUserWidget));

    public string Username
    {
      get => (string) GetValue(UsernameProperty);
      set => SetValue(UsernameProperty, value);
    }

    private void Close()
    {
      if (IsDropDownOpen == false)
      {
        return;
      }

      IsDropDownOpen = false;
    }
  }
}