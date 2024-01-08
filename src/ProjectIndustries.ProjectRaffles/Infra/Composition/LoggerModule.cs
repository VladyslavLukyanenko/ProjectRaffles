using System;
using Autofac;
using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace ProjectIndustries.ProjectRaffles.Infra.Composition
{
  public class LoggerModule : Module
  {
    private readonly string _environment;

    public LoggerModule(string environment)
    {
      _environment = environment;
    }

    protected override void Load(ContainerBuilder builder)
    {
      builder.Register(ctx =>
      {
        Log.Logger = new LoggerConfiguration()
          .Enrich.FromLogContext()
          .Enrich.With(new UserInfoEnricher(ctx.Resolve<IIdentityService>(), ctx.Resolve<ILicenseKeyProvider>()))
          .Enrich.WithProperty("environment", _environment)
          .Enrich.WithMachineName()
          .Enrich.WithEnvironmentUserName()
          .ReadFrom.Configuration(ctx.Resolve<IConfiguration>())
          .WriteTo.Elasticsearch(
            new ElasticsearchSinkOptions(
              new CloudConnectionPool(
                "project-industries:dXMtZWFzdC0xLmF3cy5mb3VuZC5pbyQ1NzE4OGNjYjIyN2I0MzlkOGY4ZTMyZmI3NDE1NDc1YyQ2ZmZjNTgzOGM3ODU0MTYyYjI5MWIxN2VjY2M2MGU5MQ==",
                new BasicAuthenticationCredentials("logs", "logs123")))
            {
              IndexFormat = "projectraffles-{0:yyyyMMdd}",
              AutoRegisterTemplate = true,
              CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true, inlineFields: true),
              EmitEventFailure =
                EmitEventFailureHandling.WriteToSelfLog |
                EmitEventFailureHandling.RaiseCallback |
                EmitEventFailureHandling.ThrowException,
              FailureCallback = e => { Console.WriteLine("Unable to submit event " + e.MessageTemplate); }
            })
          .CreateLogger();
        return new SerilogLoggerProvider();
      });

      builder
        .Register(_ => new LoggerFactory(new ILoggerProvider[] {_.Resolve<SerilogLoggerProvider>()}))
        .As<ILoggerFactory>()
        .SingleInstance();

      builder.RegisterGeneric(typeof(Logger<>))
        .As(typeof(ILogger<>))
        .SingleInstance();
    }


    private class UserInfoEnricher : ILogEventEnricher
    {
      private readonly IIdentityService _idSrv;
      private readonly ILicenseKeyProvider _lsvc;

      public UserInfoEnricher(IIdentityService idSrv, ILicenseKeyProvider lsvc)
      {
        _idSrv = idSrv;
        _lsvc = lsvc;
      }

      public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
      {
        User currentUser = _idSrv?.CurrentUser;
        string licenseKey = _lsvc?.CurrentLicenseKey;
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("licenseKey", licenseKey));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("user", currentUser, true));
      }
    }
  }
}