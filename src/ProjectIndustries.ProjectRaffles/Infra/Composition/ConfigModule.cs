using System;
using System.IO;
using System.Linq;
using Autofac;
using Microsoft.Extensions.Configuration;
using ProjectIndustries.ProjectRaffles.Core;

namespace ProjectIndustries.ProjectRaffles.Infra.Composition
{
  public class ConfigModule : Module
  {
    private readonly string _environment;

    public ConfigModule(string environment)
    {
      _environment = environment;
    }

    protected override void Load(ContainerBuilder container)
    {
      var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        .AddJsonFile($"appsettings.{_environment}.json", true)
        .AddJsonFile("appsettings.local.json", true)
        .AddEnvironmentVariables();

      var args = Environment.GetCommandLineArgs();
      if (args.Any())
      {
        builder.AddCommandLine(args);
      }

      var config = builder.Build();
      container.RegisterInstance(config)
        .SingleInstance();

      container.RegisterInstance(config)
        .As<IConfiguration>()
        .SingleInstance();


      var storageLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ProjectRaffles");
      var cfg = new ApplicationConfig
      {
        StorageLocation = storageLocation,
        Security =
        {
          ReauthenticateInternalMillis = (int) TimeSpan.FromMinutes(1).TotalMilliseconds
        },
        ProjectIndustriesApi =
        {
//#if !DEBUG
          ApiHostName = "https://api.projectindustries.gg"
//#else
  //ApiHostName = "http://localhost:3000"
//#endif
        },
        ConnectionStrings =
        {
          LiteDb = Path.Combine(storageLocation, "ProjectRaffles.db")
        }
      };

      container.RegisterInstance(cfg);
      container.RegisterInstance(cfg.Security);
      container.RegisterInstance(cfg.ProjectIndustriesApi);
      container.RegisterInstance(cfg.ConnectionStrings);
    }
  }
}