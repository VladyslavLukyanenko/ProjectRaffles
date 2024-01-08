using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Captchas
{
  public class HardcodedCaptchaResolversProvider : ICaptchaResolversProvider
  {
    public HardcodedCaptchaResolversProvider(ICaptchaRepository captchaRepository)
    {
      captchaRepository.Items.Connect()
        .AutoRefreshOnObservable(_ =>
          Observable.FromEventPattern(
              h => _.KeysCountChanged += h,
              h => _.KeysCountChanged -= h)
        )
        .AutoRefreshOnObservable(_ =>
          Observable.FromEventPattern(
            h => _.KeysBalanceChanged += h,
            h => _.KeysBalanceChanged -= h)
          )
        .DisposeMany()
        .Subscribe(providers =>
        {
          foreach (var resolver in SupportedCaptchaResolvers)
          {
            var provider = providers.Select(_ => _.Current)
              .FirstOrDefault(p => p?.ProviderName == resolver.Name);
            resolver.IsActive = !provider?.IsEmpty ?? false;
          }
        });
    }

    public IReadOnlyList<CaptchaResolverDescriptor> SupportedCaptchaResolvers { get; } =
      new List<CaptchaResolverDescriptor>
      {
        new CaptchaResolverDescriptor(SupportedCaptchaNames.TwoCaptcha),
        new CaptchaResolverDescriptor(SupportedCaptchaNames.AntiCaptcha),
        new CaptchaResolverDescriptor(SupportedCaptchaNames.CapmonsterCloud),
      };
  }
}