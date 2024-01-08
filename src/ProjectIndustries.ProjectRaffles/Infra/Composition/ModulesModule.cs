using System.Linq;
using System.Net.Http;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using LiteDB;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.Modules;
using Module = Autofac.Module;

namespace ProjectIndustries.ProjectRaffles.Infra.Composition
{
  public class ModulesModule : Module
  {
    protected override void Load(ContainerBuilder container)
    {
      container.RegisterType<CachingInterceptor>()
        .Named<IInterceptor>(nameof(CachingInterceptor));

      container.RegisterType<DistributedCacheService>()
        .Named<ICacheService>("cache");

      container.RegisterDecorator<ICacheService>(
        (ctx, impl) => new LocalCacheService(ctx.Resolve<ILiteDatabase>(), impl), fromKey: "cache");

      
      var startModulesNs = typeof(RaffleModuleNameAttribute).Namespace;
      var moduleSupportTypes = typeof(IRaffleModule)
        .Assembly
        .ExportedTypes
        .Where(t => t.Namespace?.StartsWith(startModulesNs ?? "") == true && !t.IsAbstract && t.GetInterfaces().Any())
        .ToArray();

      foreach (var moduleType in moduleSupportTypes)
      {
        var enabledInterceptor =
          moduleType.GetInterfaces().Any(i => i.GetCustomAttribute<EnableCachingAttribute>() != null);
        var builder = container.RegisterType(moduleType)
          .AsImplementedInterfaces();

        if (!enabledInterceptor)
        {
          builder.AsSelf();
        }

        builder.InstancePerDependency();
        if (enabledInterceptor)
        {
          builder
            .EnableInterfaceInterceptors()
            .InterceptedBy(nameof(CachingInterceptor));
        }
      }

      container.Register(ctx => new HttpClient())
        .InstancePerDependency();
      foreach (var module in RaffleModulesProvider.Modules)
      {
        container.RegisterType(module.ModuleType)
          .AsSelf()
          .AsImplementedInterfaces()
          .InstancePerDependency();
      }
    }
  }
}