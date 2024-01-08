using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using CefSharp.BrowserSubprocess;
using CefSharp.Wpf;
using Elastic.Apm;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProjectIndustries.ProjectRaffles.Core;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.ViralSweep;
using ReactiveUI;
using Sentry;
using Sentry.Protocol;
using Serilog;
using Serilog.Context;
using Splat;
using Splat.Autofac;
using Cef = CefSharp.Cef;

namespace ProjectIndustries.ProjectRaffles.WpfUI
{
  public static class Program
  {
    /// <summary>
    /// Application Entry Point.
    /// </summary>
    [STAThread]
    public static int Main(string[] args)
    {
      //For Windows 7 and above, app.manifest entries will take precedences of this call
      Cef.EnableHighDPISupport();

      //We are using our current exe as the BrowserSubProcess
      //Multiple instances will be spawned to handle all the 
      //Chromium proceses, render, gpu, network, plugin, etc.
      var subProcessExe = new BrowserSubprocessExecutable();
      var result = subProcessExe.Main(args);
      if (result > 0)
      {
        return result;
      }

      //We use our current exe as the BrowserSubProcess
      // var exePath = Process.GetCurrentProcess().MainModule!.FileName;

      var cachePath = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location)!, "CefSharp", "Cache");
      if (!Directory.Exists(cachePath))
      {
        Directory.CreateDirectory(cachePath);
      }

      var settings = new CefSettings
      {
        //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
        CachePath = cachePath,
        // BrowserSubprocessPath = exePath
      };

      //We are using our current exe as the BrowserSubProcess
      //Multiple instances will be spawned to handle all the 
      //Chromium proceses, render, gpu, network, plugin, etc.

      //Example of setting a command line argument
      //Enables WebRTC
      // settings.CefCommandLineArgs.Add("enable-media-stream");

      //Perform dependency check to make sure all relevant resources are in our output directory.
      Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

      using (SentrySdk.Init(sentryOptions =>
      {
        sentryOptions.Dsn = new Dsn("https://3324bac60a3a4d73ad71b3b764b19932@o389212.ingest.sentry.io/5453141");
        sentryOptions.Release = "projectraffles@" + AppConstants.CurrentAppVersion;
        sentryOptions.BeforeSend = sentryEvent =>
        {
          var idSrv = Locator.Current.GetService<IIdentityService>();
          var user = idSrv.CurrentUser;

          if (user != null)
          {
            sentryEvent.User = new User
            {
              Username = user.Username,
              Id = user.Id.ToString()
            };
          }

          return sentryEvent;
        };
      }))
      {
        var app = new App();
        try
        {
          InitializeIoC(app);

          app.InitializeComponent();
          return app.Run();
        }
        catch (Exception exc)
        {
          SentrySdk.CaptureException(exc);
          Log.Error(exc, "Error occurred");
          throw;
        }
        finally
        {
          Locator.Current.GetService<IApmAgent>()?.Flush().GetAwaiter().GetResult();
          Log.CloseAndFlush();
        }
      }
    }

    private static void InitializeIoC(Application app)
    {
      CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
      CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

      JsonConvert.DefaultSettings = () => new JsonSerializerSettings
      {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        DateFormatHandling = DateFormatHandling.IsoDateFormat,
        DateParseHandling = DateParseHandling.DateTimeOffset,
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
      };

      var timer = Stopwatch.StartNew();
      var componentsTimer = Stopwatch.StartNew();
      var container = new ContainerBuilder();
      container.RegisterViewModels()
        .RegisterApplicationServices()
        .UseAutofacDependencyResolver();

      SetupGlobalLoggingContext();
      var logger = Locator.Current.GetService<ILogger<App>>();
      ConfigureGlobalErrorsLogging(logger, app);

      Email.Materializer = Locator.Current.GetService<ICatchAllEmailMaterializer>();

      logger.LogTrace("Autofac initialized in {Elapsed}", componentsTimer.Elapsed);

      IMutableDependencyResolver current = Locator.CurrentMutable;
      componentsTimer.Restart();
      current.InitializeSplat();
      logger.LogTrace("Splat initialized in {Elapsed}", componentsTimer.Elapsed);
      componentsTimer.Restart();
      current.InitializeReactiveUI();
      logger.LogTrace("ReactiveUI initialized in {Elapsed}", componentsTimer.Elapsed);
      componentsTimer.Restart();
      current.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());
      componentsTimer.Stop();
      logger.LogTrace("Registered views for viewmodels in {Elapsed}", componentsTimer.Elapsed);

      _ = PreloadAsync(Locator.Current.GetService<IPreloadService>());

      Resolve<IRPCManager>().UpdateState("Cooking Raffles!");

      timer.Stop();
      logger.LogTrace("Application IoC/Infra initialized in {Elapsed}", timer.Elapsed);
    }


    private static void SetupGlobalLoggingContext()
    {
      LogContext.PushProperty("culture", CultureInfo.CurrentCulture.Name);
      LogContext.PushProperty("uiCulture", CultureInfo.CurrentUICulture.Name);


      LogContext.PushProperty("sessionId", Guid.NewGuid().ToString("N"));
      LogContext.PushProperty("applicationVersion", AppConstants.CurrentAppVersion);
      LogContext.PushProperty("framework", RuntimeInformation.FrameworkDescription);
      LogContext.PushProperty("os", RuntimeInformation.OSDescription);
      LogContext.PushProperty("arch", RuntimeInformation.OSArchitecture);
      LogContext.PushProperty("rid", RuntimeInformation.RuntimeIdentifier);
      var cfg = Locator.Current.GetService<ConnectionStringsConfig>();
      LogContext.PushProperty("database", cfg.LiteDb);
    }

    private static async Task PreloadAsync(IPreloadService preloadService)
    {
      await preloadService.PreAuthPreloadAsync();
    }

    private static T Resolve<T>()
    {
      return Locator.Current.GetService<T>();
    }

    private static void ConfigureGlobalErrorsLogging(ILogger<App> logger, Application app)
    {
      app.DispatcherUnhandledException += (sender, e) =>
      {
        if (e.Exception is OperationCanceledException opCExc)
        {
          MessageBox.Show(opCExc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else if (e.Exception is InvalidOperationException invOpEx)
        {
          MessageBox.Show(invOpEx.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
#if !DEBUG
          SentrySdk.CaptureException(e.Exception);
#endif
          logger.LogCritical(e.Exception, "An error occured. Handled: {Handled}. Message: {Message}", e.Handled,
            e.Exception.Message);
          MessageBox.Show(
            // "We are sorry. An error occurred. Please contact our technical support team to request assistance.",
            e.Exception.GetBaseException().Message,
            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        e.Handled = true;
      };
    }
  }
}