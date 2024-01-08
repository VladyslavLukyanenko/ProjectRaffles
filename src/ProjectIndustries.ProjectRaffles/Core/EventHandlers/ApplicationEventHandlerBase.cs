using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.EventHandlers
{
  public abstract class ApplicationEventHandlerBase<T> : IApplicationEventHandler
  {
    public Type SupportedEventType { get; } = typeof(T);
    public Task HandleAsync(object eventMessage, CancellationToken ct = default)
    {
      return HandleAsync((T) eventMessage, ct);
    }

    protected abstract Task HandleAsync(T @event, CancellationToken ct);
  }
}