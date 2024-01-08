using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Captchas;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Captchas
{
  public class AddCaptchaViewModel : ViewModelBase
  {
    private readonly ICaptchaRepository _repository;
    private readonly IToastNotificationManager _toasts;
    private readonly ICaptchaSolverFactory _captchaSolverFactory;

    public AddCaptchaViewModel(ICaptchaRepository repository, IToastNotificationManager toasts,
      ICaptchaResolversProvider captchaResolversProvider, ICaptchaSolverFactory captchaSolverFactory)
    {
      _repository = repository;
      _toasts = toasts;
      _captchaSolverFactory = captchaSolverFactory;
      Resolvers = captchaResolversProvider.SupportedCaptchaResolvers;
      
      CloseCommand = ReactiveCommand.Create(Reset);

      var canAdd = this.WhenAnyValue(_ => _.SelectedResolver)
        .CombineLatest(this.WhenAnyValue(_ => _.ApiKey), (r, k) => r != null && !string.IsNullOrWhiteSpace(k));
      AddCommand = ReactiveCommand.CreateFromTask(AddApiKeyAsync, canAdd);
    }

    private async Task AddApiKeyAsync(CancellationToken ct)
    {
      var provider = await _repository.GetProviderByName(SelectedResolver.Name, ct);
      if (provider == null)
      {
        provider = new CaptchaProvider(SelectedResolver.Name);
      }

      var key = new CaptchaKey(ApiKey);
      var solver = _captchaSolverFactory.Create(provider, key);
      try
      {
        key.Balance = await solver.GetBalance(ct);
      }
      catch
      {
        _toasts.Show(ToastContent.Error("Invalid captcha key provided."));
        return;
      }
      
      provider.AddKey(key);
      await _repository.SaveAsync(provider, ct);
      _toasts.Show(ToastContent.Success("API key was added"));
      CloseCommand.Execute().Subscribe();
    }

    [Reactive] public string ApiKey { get; set; }
    [Reactive] public CaptchaResolverDescriptor SelectedResolver { get; set; }
    public IEnumerable<CaptchaResolverDescriptor> Resolvers { get; }

    public ReactiveCommand<Unit, Unit> AddCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> CloseCommand { get; private set; }

    public void Reset()
    {
      ApiKey = null;
      SelectedResolver = null;
    }
  }
}