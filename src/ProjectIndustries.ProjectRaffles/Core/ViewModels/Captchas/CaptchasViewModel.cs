using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Captchas
{
  public class CaptchasViewModel : ViewModelBase
  {
    private readonly ReadOnlyObservableCollection<CaptchaRowViewModel> _rows;

    public CaptchasViewModel(ICaptchaRepository captchaRepository, ICaptchaSolverFactory factory,
      IToastNotificationManager toasts)
    {
      captchaRepository.Items.Connect()
        .AutoRefreshOnObservable(_ => Observable.FromEventPattern(
          h => _.KeysCountChanged += h,
          h => _.KeysCountChanged -= h)
        )
        .TransformMany(
          p => p.Keys.Select(key => new CaptchaRowViewModel(p, key, captchaRepository, factory, toasts)),
          _ => _.Provider.ProviderName + _.Key.Value
        )
        .Bind(out _rows)
        .DisposeMany()
        .Subscribe();
    }

    public ReadOnlyObservableCollection<CaptchaRowViewModel> Rows => _rows;
  }
}