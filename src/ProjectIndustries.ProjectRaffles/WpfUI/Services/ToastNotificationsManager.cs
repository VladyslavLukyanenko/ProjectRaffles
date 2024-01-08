using System;
using System.Collections.Generic;
using System.Windows;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Lifetime.Clear;
using ToastNotifications.Messages.Error;
using ToastNotifications.Messages.Information;
using ToastNotifications.Messages.Success;
using ToastNotifications.Messages.Warning;
using ToastNotifications.Position;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services
{
  public class ToastNotificationsManager : IToastNotificationManager
  {
    private Notifier _notifier;
    private bool _isSuspended;
    private readonly List<ToastContent> _suspendedContent = new List<ToastContent>();

    public ToastNotificationsManager()
    {
      Init();
      App.Current.Exit += (sender, args) =>
      {
        Suspend();
        Clear();
        _notifier?.Dispose();
      };
    }

    public void Suspend()
    {
      _isSuspended = true;
    }

    public void Resume()
    {
      Clear();
      _isSuspended = false;
      foreach (var toastContent in _suspendedContent)
      {
        Show(toastContent);
      }
    }

    public void Clear()
    {
      _notifier?.Dispose();

      Init();
      _notifier.ClearMessages(new ClearAll());
    }

    private void Init()
    {
      _notifier = new Notifier(cfg =>
      {
        cfg.PositionProvider = new WindowPositionProvider(
          parentWindow: Application.Current.MainWindow,
          corner: Corner.TopLeft,
          offsetX: 10,
          offsetY: 10);

        cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
          notificationLifetime: TimeSpan.FromSeconds(3),
          maximumNotificationCount: MaximumNotificationCount.FromCount(5));

        cfg.Dispatcher = Application.Current.Dispatcher;
      });
    }

    public void Show(ToastContent content)
    {
      if (_isSuspended)
      {
        _suspendedContent.Add(content);
        return;
      }

      try
      {
        App.Current.Dispatcher.Invoke(() =>
        {
          _notifier.Notify<INotification>(() => content.Type switch
          {
            ToastType.Error => new ErrorMessage(content.Content),
            ToastType.Information => new InformationMessage(content.Content),
            ToastType.Success => new SuccessMessage(content.Content),
            ToastType.Warning => new WarningMessage(content.Content),
            _ => throw new ArgumentOutOfRangeException()
          });
        });
      }
      catch
      {
        // noop
      }
    }
  }
}