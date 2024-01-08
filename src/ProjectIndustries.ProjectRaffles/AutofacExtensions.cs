using System;
using Autofac;
using ProjectIndustries.ProjectRaffles.Core.Clients;
using ProjectIndustries.ProjectRaffles.Core.EventHandlers;
using ProjectIndustries.ProjectRaffles.Core.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks;
using ProjectIndustries.ProjectRaffles.Core.Validators;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;
using ProjectIndustries.ProjectRaffles.Infra.Composition;
using ProjectIndustries.ProjectRaffles.WpfUI.Services;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles
{
  public static class AutofacExtensions
  {
#if DEBUG
    const string FallbackEnvironmentName = "Development";
#else
      const string FallbackEnvironmentName = "Production";
#endif
    private static readonly string EnvironmentName =
      Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? FallbackEnvironmentName;


    public static ContainerBuilder RegisterViewModels(this ContainerBuilder container)
    {
      container.RegisterAssemblyTypes(typeof(ViewModelBase).Assembly)
        .AssignableTo<ViewModelBase>()
        .AsSelf()
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();

      container.RegisterInstance(new RoutingState())
        .AsSelf();

      return container;
    }

    public static ContainerBuilder RegisterApplicationServices(this ContainerBuilder container)
    {
      container.RegisterAssemblyTypes(typeof(IRaffleModule).Assembly)
        .Where(_ => !_.IsAbstract && !_.IsNested)
        .AsImplementedInterfaces()
        .InNamespaceOf<MailchimpService>()
        .Except<MailKitMailsWatcher>()
        .Except<MailKitRaffleMailsWatcher>()
        .Except<PendingRaffleTaskStatusWatcher>()
        .SingleInstance();

      container.RegisterTypes(typeof(MailKitMailsWatcher), typeof(MailKitRaffleMailsWatcher),
          typeof(PendingRaffleTaskStatusWatcher))
        .AsImplementedInterfaces()
        .InstancePerDependency();

      container.RegisterAssemblyTypes(typeof(ILicenseKeyApiClient).Assembly)
        .Where(_ => !_.IsAbstract)
        .AsImplementedInterfaces()
        .InNamespaceOf<ILicenseKeyApiClient>()
        .SingleInstance();

      container.RegisterAssemblyTypes(typeof(DiscordSettingsValidator).Assembly)
        .Where(_ => !_.IsAbstract)
        .AsSelf()
        .InNamespaceOf<DiscordSettingsValidator>()
        .SingleInstance();

      container.RegisterAssemblyTypes(typeof(ViewModelBase).Assembly)
        .AssignableTo<IApplicationEventHandler>()
        .As<IApplicationEventHandler>()
        .SingleInstance();

      container.RegisterAssemblyTypes(typeof(AutofacExtensions).Assembly)
        .Where(_ => !_.IsAbstract)
        .AsImplementedInterfaces()
        .InNamespaceOf<IWindowFactory>()
        .SingleInstance();

      container.RegisterType<MessageBus>().AsImplementedInterfaces().SingleInstance();

      container.RegisterModule(new ConfigModule(EnvironmentName));
      container.RegisterModule(new LiteDbModule());
      container.RegisterModule(new LoggerModule(EnvironmentName));
      container.RegisterModule(new ModulesModule());
      container.RegisterModule(new AutoMapperModule());
      container.RegisterModule(new ApmModule(EnvironmentName));

      return container;
    }
  }
}