using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Logging;
using ProjectIndustries.ProjectRaffles.Core.EventHandlers;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public class ApplicationEventsManager : IApplicationEventsManager
  {
    private static readonly MethodInfo SubscribeHandlerMethod;

    static ApplicationEventsManager()
    {
      SubscribeHandlerMethod = ReflectionHelper
        .GetGenericMethodDef<ApplicationEventsManager>(_ => _.SubscribeHandler<object>(null));
    }
    
    
    private readonly IMessageBus _messageBus;
    private readonly IList<IApplicationEventHandler> _eventHandlers;
    private readonly ILogger<ApplicationEventsManager> _logger;

    public ApplicationEventsManager(IMessageBus messageBus, IList<IApplicationEventHandler> eventHandlers,
      ILogger<ApplicationEventsManager> logger)
    {
      _messageBus = messageBus;
      _eventHandlers = eventHandlers;
      _logger = logger;
    }

    public void Spawn()
    {
      _logger.LogDebug("Registering application event listeners");
      foreach (var h in _eventHandlers)
      {
        // ReSharper disable once PossibleNullReferenceException
        var genericMethod = SubscribeHandlerMethod.MakeGenericMethod(h.SupportedEventType);
        genericMethod.Invoke(this, new object[] {h});
      }

      _logger.LogDebug("Listeners are registered");
    }

    private void SubscribeHandler<T>(ApplicationEventHandlerBase<T> handler)
    {
      _messageBus.Listen<T>()
        .Subscribe(m => { handler.HandleAsync(m); });
      _logger.LogDebug($"Registered {handler.GetType().Name}");
    }
  }
}