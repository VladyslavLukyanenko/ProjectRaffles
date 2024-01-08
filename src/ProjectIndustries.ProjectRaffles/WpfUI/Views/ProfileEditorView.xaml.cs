using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Views
{
  /// <summary>
  /// Interaction logic for ProfileEditorView.xaml
  /// </summary>
  public partial class ProfileEditorView
  {
    public ProfileEditorView()
    {
      InitializeComponent();
      this.WhenAnyValue(_ => _.ViewModel).BindTo(this, _ => _.DataContext);
      this.WhenActivated(d =>
      {
        Observable.FromEventPattern(
            (EventHandler<RoutedEventArgs> ev) => new RoutedEventHandler(ev),
            handler => pwdCvv.PasswordChanged += handler,
            handler => pwdCvv.PasswordChanged -= handler)
          .Where(_ => ViewModel!.Profile.CreditCard != null)
          .Throttle(TimeSpan.FromMilliseconds(300))
          .Select(_ => SecureStringToString(pwdCvv.SecurePassword))
          .DistinctUntilChanged()
          .ObserveOn(RxApp.MainThreadScheduler)
          .Subscribe(pwd => ViewModel!.Profile.CreditCard.Cvv = pwd)
          .DisposeWith(d);

        IDisposable cvvChanged = null;
        ViewModel.WhenAnyValue(_ => _.Profile)
          .Subscribe(p =>
          {
            cvvChanged?.Dispose();
            cvvChanged = p.WhenAnyValue(_ => _.CreditCard)
              .Where(c => c != null)
              .Select(_ => _.Cvv)
              .DistinctUntilChanged()
              .Where(cvv => cvv != SecureStringToString(pwdCvv.SecurePassword))
              .Subscribe(cvv => pwdCvv.Password = cvv);
          })
          .DisposeWith(d);
      });
    }

    private string SecureStringToString(SecureString value)
    {
      IntPtr valuePtr = IntPtr.Zero;
      try
      {
        valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
        return Marshal.PtrToStringUni(valuePtr);
      }
      finally
      {
        Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
      }
    }
  }
}