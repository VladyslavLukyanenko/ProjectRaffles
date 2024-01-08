using System;
using System.Reactive.Linq;
using System.Windows;
using Microsoft.Extensions.Logging;
using ProjectIndustries.ProjectRaffles.Core;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;
using ProjectIndustries.ProjectRaffles.WpfUI.Views;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services
{
  public class SecurityManager
    : ISecurityManager
  {
    private readonly IIdentityService _identityService;
    private readonly IWindowFactory _windowFactory;
    private readonly ILogger<SecurityManager> _logger;
    private readonly SecurityConfig _securityConfig;
    private Window _currentWindow;

    public SecurityManager(IIdentityService identityService, SecurityConfig securityConfig,
      IWindowFactory windowFactory, ILogger<SecurityManager> logger)
    {
      _identityService = identityService;
      _securityConfig = securityConfig;
      _windowFactory = windowFactory;
      _logger = logger;
    }

    public void Spawn()
    {
      _logger.LogDebug("Spawning security manager");
      _identityService.IsAuthenticated
        .DistinctUntilChanged()
        .ObserveOn(RxApp.MainThreadScheduler)
        .Subscribe(isAuthenticated =>
        {
          _logger.LogDebug("Authentication state changed");
          Window prevWnd = _currentWindow;
          if (isAuthenticated)
          {
            _logger.LogDebug("User is authenticated");
            _currentWindow = _windowFactory.CreateWindow<MainWindowView, MainWindowViewModel>();
          }
          else
          {
            _logger.LogDebug("User isn't authenticated");
            if (App.Current.MainWindow?.GetType() == typeof(LoginScreenView))
            {
              _logger.LogDebug("Login screen already started");
              _currentWindow = App.Current.MainWindow;
              return;
            }

            _currentWindow = _windowFactory.CreateWindow<LoginScreenView, LoginScreenViewModel>();
          }

          _logger.LogDebug("Showing window");
          Application.Current.MainWindow = _currentWindow;
          _currentWindow.Show();
          prevWnd?.Close();
          _logger.LogDebug("Window shown");
        });

      _logger.LogDebug("Started authentication check by interval");
      // var isNotAuthenticated = _identityService.IsAuthenticated.Select(isAuthenticated => !isAuthenticated);
      Observable.Interval(TimeSpan.FromMilliseconds(_securityConfig.ReauthenticateInternalMillis),
          RxApp.TaskpoolScheduler)
        .Subscribe(async _ => { await _identityService.TryAuthenticateAsync(); });
    }
  }
}