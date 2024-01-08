using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ProjectIndustries.ProjectRaffles.Core.Domain.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Captchas
{
  public class CaptchaSolveService : ICaptchaSolveService
  {
    private readonly ICaptchaRepository _captchaRepository;
    private readonly ICaptchaSolverFactory _captchaSolverFactory;
    private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);
    private readonly ReadOnlyObservableCollection<CaptchaProvider> _captchaProviders;

    public CaptchaSolveService(ICaptchaRepository captchaRepository, ICaptchaSolverFactory captchaSolverFactory)
    {
      _captchaRepository = captchaRepository;
      _captchaSolverFactory = captchaSolverFactory;
      _captchaRepository.Items.Connect()
        .Bind(out _captchaProviders)
        .Subscribe();
    }

    public Task<string> SolveReCaptchaV2Async(string sitekey, string pageUrl, bool isVisible,
      CancellationToken ct)
    {
      return SolveAsync(solver => solver.SolveReCaptchaV2Async(sitekey, pageUrl, isVisible, ct), ct);
    }

    public Task<string> SolveImageCaptchaAsync(string base64Image, CancellationToken ct)
    {
      return SolveAsync(solver => solver.SolveImageCaptchaAsync(base64Image, ct), ct);
    }

    public Task<string> SolveReCaptchaV3Async(string sitekey, string pageUrl, string action, double minScore,
      CancellationToken ct)
    {
      return SolveAsync(solver => solver.SolveReCaptchaV3Async(sitekey, pageUrl, action, minScore, ct), ct);
    }


    private async Task<string> SolveAsync(Func<ICaptchaSolver, Task<CaptchaResult>> solveExecutor,
      CancellationToken ct)
    {
      foreach (var provider in _captchaProviders.OrderBy(_ => _.MostIdleKeyUsageTimes))
      {
        CaptchaKey usedKey = null;
        try
        {
          ICaptchaSolver solver;
          try
          {
            await SemaphoreSlim.WaitAsync(ct);

            solver = _captchaSolverFactory.Create(provider, out usedKey);
            usedKey.Used();
          }
          finally
          {
            SemaphoreSlim.Release();
          }

          var result = await solveExecutor(solver);
          if (!result.Success)
          {
            throw new InvalidOperationException("Can't solve captcha");
          }

          await _captchaRepository.SaveAsync(provider, ct);
          return result.Response;
        }
        catch
        {
          usedKey?.FailedToUse();
          await _captchaRepository.SaveAsync(provider, ct);
        }
      }

      throw new InvalidOperationException("Can't solve captcha. All providers failed");
    }

    // private readonly IGeneralSettingsService _settings;
    //
    // public CaptchaSolveProvider(IGeneralSettingsService settings)
    // {
    //   _settings = settings;
    // }
    //
    // public async Task<string> GetBalance(CancellationToken ct)
    // {
    //   var balanceClient = new _2CaptchaSolver(_settings.CurrentSettings.GetCaptchaApiKey("2Captcha"));
    //   var balance = await balanceClient.GetBalance(ct);
    //
    //   return balance.Response;
    // }
    //
    // public async Task<string> SolveImageCaptchaAsync(string b64image, CancellationToken ct)
    // {
    //   var captcha = new _2CaptchaSolver(_settings.CurrentSettings.GetCaptchaApiKey("2Captcha"));
    //   var image = await captcha.SolveImageCaptchaAsync(b64image, ct);
    //
    //   return image.Response;
    // }
    //
    // public async Task<string> SolveReCaptchaV2Async(string sitekey, string url, bool isVisible, CancellationToken ct)
    // {
    //   var captcha = new _2CaptchaSolver(_settings.CurrentSettings.GetCaptchaApiKey("2Captcha"));
    //   var reCaptchaInvisible = await captcha.SolveReCaptchaV2Async(sitekey, url, isVisible, ct);
    //
    //   return reCaptchaInvisible.Response;
    // }
  }
}