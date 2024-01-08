using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public class NotificationsService : INotificationsService
  {
    private const int CountLimit = 20;
    private readonly SemaphoreSlim _gate = new SemaphoreSlim(1, 1);

    private readonly BehaviorSubject<LinkedList<NotificationViewModel>> _notifications =
      new BehaviorSubject<LinkedList<NotificationViewModel>>(new LinkedList<NotificationViewModel>());

    public NotificationsService()
    {
      Notifications = _notifications
        .AsObservable()
        .Synchronize(this)
        .Select(l => l.ToList())
        .Publish()
        .RefCount();
    }

    public IObservable<IEnumerable<NotificationViewModel>> Notifications { get; }

    public void Add(NotificationViewModel notification)
    {
      try
      {
        _gate.Wait();
        var list = _notifications.Value;
        list.AddFirst(notification);
        if (list.Count >= CountLimit)
        {
          list.RemoveLast();
        }

        _notifications.OnNext(list);
      }
      finally
      {
        _gate.Release();
      }
    }

    public void Clear()
    {
      _notifications.OnNext(new LinkedList<NotificationViewModel>());
    }
  }
}