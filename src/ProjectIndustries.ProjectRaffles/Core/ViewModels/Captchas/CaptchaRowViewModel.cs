using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Captchas;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Captchas
{
  public class CaptchaRowViewModel : ViewModelBase, IDisposable
  {
    private const decimal IsLowBalanceTreshold = 1m;
    private readonly ICaptchaRepository _repository;
    private readonly IToastNotificationManager _toasts;
    private readonly CompositeDisposable _disposable = new CompositeDisposable();
    private readonly ICaptchaSolver _solver;

    public CaptchaRowViewModel(CaptchaProvider provider, CaptchaKey key, ICaptchaRepository repository,
      ICaptchaSolverFactory factory, IToastNotificationManager toasts)
    {
      _repository = repository;
      _toasts = toasts;
      Provider = provider;
      Key = key;
      _solver = factory.Create(provider, key);

      RemoveCommand = ReactiveCommand.CreateFromTask(RemoveKeyAsync);

      Observable.Interval(TimeSpan.FromSeconds(30), RxApp.TaskpoolScheduler)
        .StartWith(0L)
        .Select(_ => Observable.FromAsync(RefreshBalanceAsync))
        .Concat()
        // .Catch<decimal, Exception>(_ => Observable.Return(0M))
        .ObserveOn(RxApp.MainThreadScheduler)
        .ToPropertyEx(this, _ => _.Balance)
        .DisposeWith(_disposable);

      Key.BalanceChanged += UpdateProviderOnBalanceChanged;

      this.WhenAnyValue(_ => _.Balance)
        .Select(b => b > IsLowBalanceTreshold)
        .DistinctUntilChanged()
        .ObserveOn(RxApp.MainThreadScheduler)
        .ToPropertyEx(this, _ => _.IsBalanceHigh);
    }

    private async void UpdateProviderOnBalanceChanged(object sender, EventArgs e)
    {
      await _repository.SaveSilentlyAsync(Provider);
    }

    private async Task<decimal> RefreshBalanceAsync(CancellationToken ct = default)
    {
      try
      {
        var balance = await _solver.GetBalance(ct);
        if (Key.Balance > IsLowBalanceTreshold && balance <= IsLowBalanceTreshold)
        {
          _toasts.Show(
            ToastContent.Warning($"Balance of some key of {Provider.ProviderName} captcha service is too low."));
        }

        Key.Balance = balance;
        return balance;
      }
      catch
      {
        return 0M;
      }
    }

    private async Task RemoveKeyAsync(CancellationToken ct)
    {
      Provider.RemoveKey(Key);
      if (Provider.IsEmpty)
      {
        await _repository.RemoveAsync(Provider, ct);
      }
      else
      {
        await _repository.SaveAsync(Provider, ct);
      }
    }

    public CaptchaProvider Provider { get; }
    public CaptchaKey Key { get; }

    public decimal Balance { [ObservableAsProperty] get; }
    public bool IsBalanceHigh { [ObservableAsProperty] get; }

    public ReactiveCommand<Unit, Unit> RemoveCommand { get; private set; }

    public void Dispose()
    {
      Key.BalanceChanged -= UpdateProviderOnBalanceChanged;
      _disposable.Dispose();
    }
  }
}