using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Win32;
using ProjectIndustries.ProjectRaffles.WpfUI.Infra;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Controls
{
  public abstract class CustomizableWindow
    : Window
  {
    private HwndSource _hwndSource;

    private bool isMouseButtonDown;
    private bool isManualDrag;
    private System.Windows.Point mouseDownPosition;
    private System.Windows.Point positionBeforeDrag;
    private System.Windows.Point previousScreenBounds;

    public FrameworkElement WindowRoot { get; private set; }
    public FrameworkElement LayoutRoot { get; private set; }
    public System.Windows.Controls.Button MinimizeButton { get; private set; }
    public System.Windows.Controls.Button MaximizeButton { get; private set; }
    public System.Windows.Controls.Button RestoreButton { get; private set; }
    public System.Windows.Controls.Button CloseButton { get; private set; }
    public Grid HeaderBar { get; private set; }
    public ContentPresenter HeaderContentPresenter { get; private set; }
    public double HeightBeforeMaximize { get; private set; }
    public double WidthBeforeMaximize { get; private set; }
    public WindowState PreviousState { get; private set; }

    /// <summary>Identifies the <see cref="P:System.Windows.Controls.Control.Template" />dependency property. </summary>
    /// <returns>The identifier for the <see cref="P:System.Windows.Controls.Control.Template" />dependency property.</returns>
    public static readonly DependencyProperty HeaderTemplateProperty =
      DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(CustomizableWindow),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));


    /// <summary>Gets or sets a control template.   </summary>
    /// <returns>The template that defines the appearance of the <see cref="T:System.Windows.Controls.Control" />.</returns>
    public DataTemplate HeaderTemplate
    {
      get => (DataTemplate) GetValue(HeaderTemplateProperty);
      set => SetValue(HeaderTemplateProperty, value);
    }

    static CustomizableWindow()
    {
    }

    public CustomizableWindow()
    {
      double currentDPIScaleFactor = (double) SystemHelper.GetCurrentDPIScaleFactor();
      Screen screen = Screen.FromHandle((new WindowInteropHelper(this)).Handle);
      base.SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);
      base.StateChanged += new EventHandler(this.OnStateChanged);
      base.Loaded += new RoutedEventHandler(this.OnLoaded);
      Rectangle workingArea = screen.WorkingArea;
      base.MaxHeight = (double) (workingArea.Height + 16) / currentDPIScaleFactor;
      SystemEvents.DisplaySettingsChanged += new EventHandler(this.SystemEvents_DisplaySettingsChanged);
      this.AddHandler(Window.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.OnMouseButtonUp), true);
      this.AddHandler(Window.MouseMoveEvent, new System.Windows.Input.MouseEventHandler(this.OnMouseMove));
    }

    protected virtual Thickness GetDefaultMarginForDpi()
    {
      int currentDPI = SystemHelper.GetCurrentDPI();
      Thickness thickness = new Thickness(8, 8, 8, 8);
      if (currentDPI == 120)
      {
        thickness = new Thickness(7, 7, 4, 5);
      }
      else if (currentDPI == 144)
      {
        thickness = new Thickness(7, 7, 3, 1);
      }
      else if (currentDPI == 168)
      {
        thickness = new Thickness(6, 6, 2, 0);
      }
      else if (currentDPI == 192)
      {
        thickness = new Thickness(6, 6, 0, 0);
      }
      else if (currentDPI == 240)
      {
        thickness = new Thickness(6, 6, 0, 0);
      }

      return thickness;
    }

    protected virtual Thickness GetFromMinimizedMarginForDpi()
    {
      int currentDPI = SystemHelper.GetCurrentDPI();
      Thickness thickness = new Thickness(7, 7, 5, 7);
      if (currentDPI == 120)
      {
        thickness = new Thickness(6, 6, 4, 6);
      }
      else if (currentDPI == 144)
      {
        thickness = new Thickness(7, 7, 4, 4);
      }
      else if (currentDPI == 168)
      {
        thickness = new Thickness(6, 6, 2, 2);
      }
      else if (currentDPI == 192)
      {
        thickness = new Thickness(6, 6, 2, 2);
      }
      else if (currentDPI == 240)
      {
        thickness = new Thickness(6, 6, 0, 0);
      }

      return thickness;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      Screen screen = Screen.FromHandle((new WindowInteropHelper(this)).Handle);
      double width = (double) screen.WorkingArea.Width;
      Rectangle workingArea = screen.WorkingArea;
      this.previousScreenBounds = new System.Windows.Point(width, (double) workingArea.Height);
    }

    private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
    {
      Screen screen = Screen.FromHandle((new WindowInteropHelper(this)).Handle);
      double width = (double) screen.WorkingArea.Width;
      Rectangle workingArea = screen.WorkingArea;
      this.previousScreenBounds = new System.Windows.Point(width, (double) workingArea.Height);
      this.RefreshWindowState();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (base.WindowState == WindowState.Normal)
      {
        this.HeightBeforeMaximize = base.ActualHeight;
        this.WidthBeforeMaximize = base.ActualWidth;
        return;
      }

      if (base.WindowState == WindowState.Maximized)
      {
        Screen screen = Screen.FromHandle((new WindowInteropHelper(this)).Handle);
        if (this.previousScreenBounds.X != (double) screen.WorkingArea.Width ||
            this.previousScreenBounds.Y != (double) screen.WorkingArea.Height)
        {
          double width = (double) screen.WorkingArea.Width;
          Rectangle workingArea = screen.WorkingArea;
          this.previousScreenBounds = new System.Windows.Point(width, (double) workingArea.Height);
          this.RefreshWindowState();
        }
      }
    }

    private void OnStateChanged(object sender, EventArgs e)
    {
      Screen screen = Screen.FromHandle((new WindowInteropHelper(this)).Handle);
      Thickness thickness = new Thickness(0);
      if (this.WindowState != WindowState.Maximized)
      {
        double currentDPIScaleFactor = (double) SystemHelper.GetCurrentDPIScaleFactor();
        Rectangle workingArea = screen.WorkingArea;
        this.MaxHeight = (double) (workingArea.Height + 16) / currentDPIScaleFactor;
        this.MaxWidth = double.PositiveInfinity;

        if (this.WindowState != WindowState.Maximized)
        {
          this.SetMaximizeButtonsVisibility(Visibility.Visible, Visibility.Collapsed);
        }
      }
      else
      {
        thickness = this.GetDefaultMarginForDpi();
        if (this.PreviousState == WindowState.Minimized ||
            this.Left == this.positionBeforeDrag.X && this.Top == this.positionBeforeDrag.Y)
        {
          thickness = this.GetFromMinimizedMarginForDpi();
        }

        this.SetMaximizeButtonsVisibility(Visibility.Collapsed, Visibility.Visible);
      }

      this.LayoutRoot.Margin = thickness;
      this.PreviousState = this.WindowState;
    }

    private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (!this.isMouseButtonDown)
      {
        return;
      }

      double currentDPIScaleFactor = (double) SystemHelper.GetCurrentDPIScaleFactor();
      System.Windows.Point position = e.GetPosition(this);
      System.Windows.Point screen = base.PointToScreen(position);
      double x = this.mouseDownPosition.X - position.X;
      double y = this.mouseDownPosition.Y - position.Y;
      if (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)) > 1)
      {
        double actualWidth = this.mouseDownPosition.X;

        if (this.mouseDownPosition.X <= 0)
        {
          actualWidth = 0;
        }
        else if (this.mouseDownPosition.X >= base.ActualWidth)
        {
          actualWidth = this.WidthBeforeMaximize;
        }

        if (base.WindowState == WindowState.Maximized)
        {
          this.ToggleWindowState();
          this.Top = (screen.Y - position.Y) / currentDPIScaleFactor;
          this.Left = (screen.X - actualWidth) / currentDPIScaleFactor;
          this.CaptureMouse();
        }

        this.isManualDrag = true;

        this.Top = (screen.Y - this.mouseDownPosition.Y) / currentDPIScaleFactor;
        this.Left = (screen.X - actualWidth) / currentDPIScaleFactor;
      }
    }


    private void OnMouseButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.isMouseButtonDown = false;
      this.isManualDrag = false;
      this.ReleaseMouseCapture();
    }

    private void RefreshWindowState()
    {
      if (base.WindowState == WindowState.Maximized)
      {
        this.ToggleWindowState();
        this.ToggleWindowState();
      }
    }


    public T GetRequiredTemplateChild<T>(string childName) where T : DependencyObject
    {
      return (T) base.GetTemplateChild(childName);
    }

    public override void OnApplyTemplate()
    {
      this.WindowRoot = this.GetRequiredTemplateChild<FrameworkElement>("WindowRoot");
      this.LayoutRoot = this.GetRequiredTemplateChild<FrameworkElement>("LayoutRoot");
      this.MinimizeButton = this.GetRequiredTemplateChild<System.Windows.Controls.Button>("MinimizeButton");
      this.MaximizeButton = this.GetRequiredTemplateChild<System.Windows.Controls.Button>("MaximizeButton");
      this.RestoreButton = this.GetRequiredTemplateChild<System.Windows.Controls.Button>("RestoreButton");
      this.CloseButton = this.GetRequiredTemplateChild<System.Windows.Controls.Button>("CloseButton");
      this.HeaderBar = this.GetRequiredTemplateChild<Grid>("PART_HeaderBar");
      this.HeaderContentPresenter = this.GetRequiredTemplateChild<ContentPresenter>("PART_HeaderContentPresenter");

      if (this.LayoutRoot != null && this.WindowState == WindowState.Maximized)
      {
        this.LayoutRoot.Margin = GetDefaultMarginForDpi();
      }

      if (this.CloseButton != null)
      {
        this.CloseButton.Click += CloseButton_Click;
      }

      if (this.MinimizeButton != null)
      {
        this.MinimizeButton.Click += MinimizeButton_Click;
      }

      if (this.RestoreButton != null)
      {
        this.RestoreButton.Click += RestoreButton_Click;
      }

      if (this.MaximizeButton != null)
      {
        this.MaximizeButton.Click += MaximizeButton_Click;
      }

      if (this.HeaderBar != null)
      {
        this.HeaderBar.AddHandler(Grid.MouseLeftButtonDownEvent,
          new MouseButtonEventHandler(this.OnHeaderBarMouseLeftButtonDown));
      }

      //if (this.HeaderContentPresenter != null)
      //{
      //  this.HeaderContentPresenter.AddHandler(ContentPresenter.MouseLeftButtonDownEvent,
      //    new MouseButtonEventHandler(this.OnHeaderBarMouseLeftButtonDown));
      //}

      base.OnApplyTemplate();
    }

    protected override void OnInitialized(EventArgs e)
    {
      SourceInitialized += OnSourceInitialized;
      base.OnInitialized(e);
    }

    protected virtual void OnHeaderBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (isManualDrag)
      {
        return;
      }

      System.Windows.Point position = e.GetPosition(this);
      int headerBarHeight = 36;
      int leftmostClickableOffset = 50;

      if (position.X - this.LayoutRoot.Margin.Left <= leftmostClickableOffset && position.Y <= headerBarHeight)
      {
        if (e.ClickCount != 2)
        {
          this.OpenSystemContextMenu(e);
        }
        else
        {
          base.Close();
        }

        e.Handled = true;
        return;
      }

      if (e.ClickCount == 2 && base.ResizeMode == ResizeMode.CanResize)
      {
        this.ToggleWindowState();
        return;
      }

      if (base.WindowState == WindowState.Maximized)
      {
        this.isMouseButtonDown = true;
        this.mouseDownPosition = position;
      }
      else
      {
        try
        {
          this.positionBeforeDrag = new System.Windows.Point(base.Left, base.Top);
          this.DragMove();
        }
        catch
        {
        }
      }
    }

    protected void ToggleWindowState()
    {
      if (base.WindowState != WindowState.Maximized)
      {
        base.WindowState = WindowState.Maximized;
      }
      else
      {
        base.WindowState = WindowState.Normal;
      }
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
      this.ToggleWindowState();
    }

    private void RestoreButton_Click(object sender, RoutedEventArgs e)
    {
      this.ToggleWindowState();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
      this.WindowState = WindowState.Minimized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void OnSourceInitialized(object sender, EventArgs e)
    {
      _hwndSource = (HwndSource) PresentationSource.FromVisual(this);
    }

    private void SetMaximizeButtonsVisibility(Visibility maximizeButtonVisibility,
      Visibility reverseMaximizeButtonVisiility)
    {
      if (this.MaximizeButton != null)
      {
        this.MaximizeButton.Visibility = maximizeButtonVisibility;
      }

      if (this.RestoreButton != null)
      {
        this.RestoreButton.Visibility = reverseMaximizeButtonVisiility;
      }
    }

    private void OpenSystemContextMenu(MouseButtonEventArgs e)
    {
      System.Windows.Point position = e.GetPosition(this);
      System.Windows.Point screen = this.PointToScreen(position);
      int num = 36;
      if (position.Y < (double) num)
      {
        IntPtr handle = (new WindowInteropHelper(this)).Handle;
        IntPtr systemMenu = NativeUtils.GetSystemMenu(handle, false);
        if (base.WindowState != WindowState.Maximized)
        {
          NativeUtils.EnableMenuItem(systemMenu, 61488, 0);
        }
        else
        {
          NativeUtils.EnableMenuItem(systemMenu, 61488, 1);
        }

        int num1 = NativeUtils.TrackPopupMenuEx(systemMenu, NativeUtils.TPM_LEFTALIGN | NativeUtils.TPM_RETURNCMD,
          Convert.ToInt32(screen.X + 2), Convert.ToInt32(screen.Y + 2), handle, IntPtr.Zero);
        if (num1 == 0)
        {
          return;
        }

        NativeUtils.PostMessage(handle, 274, new IntPtr(num1), IntPtr.Zero);
      }
    }
  }
}