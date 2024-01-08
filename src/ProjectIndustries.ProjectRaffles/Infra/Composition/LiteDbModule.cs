using System;
using System.IO;
using Autofac;
using LiteDB;
using ProjectIndustries.ProjectRaffles.Core;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Captchas;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Services.PaymentProcessors;

namespace ProjectIndustries.ProjectRaffles.Infra.Composition
{
  public class LiteDbModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder.Register(ctx =>
        {
          var cfg = ctx.Resolve<ConnectionStringsConfig>();
          var dir = Path.GetDirectoryName(cfg.LiteDb)
                    ?? throw new InvalidOperationException($"Cannot resolve directory name from path '{cfg.LiteDb}'");
          if (!Directory.Exists(dir))
          {
            Directory.CreateDirectory(dir);
          }

          ConfigureLiteDbMapper();

          var liteDatabase = new LiteDatabase(cfg.LiteDb)
          {
            UtcDate = true
          };

          return liteDatabase;
        })
        .AsImplementedInterfaces()
        .SingleInstance();
    }

    private static void ConfigureLiteDbMapper()
    {
      var mapper = BsonMapper.Global;
      mapper.UseCamelCase();

      mapper.Entity<Profile>()
        .Ignore(_ => _.Changed)
        .Ignore(_ => _.Changing)
        .Ignore(_ => _.ThrownExceptions);

      mapper.Entity<CreditCard>()
        .Ignore(_ => _.Changed)
        .Ignore(_ => _.Changing)
        .Ignore(_ => _.ThrownExceptions);

      mapper.Entity<Address>()
        .Ignore(_ => _.FullName)
        .Ignore(_ => _.Changed)
        .Ignore(_ => _.Changing)
        .Ignore(_ => _.ThrownExceptions);

      mapper.Entity<CaptchaProvider>()
        .Ignore(_ => _.IsEmpty)
        .Ignore(_ => _.MostIdleKeyUsageTimes);

      mapper.Entity<Email>();
      mapper.Entity<Account>();

      mapper.Entity<ProxyGroup>()
        .Ignore(_ => _.HasAnyProxy);

      mapper.Entity<Proxy>()
        .Ignore(_ => _.IsAvailable);
    }
  }
}