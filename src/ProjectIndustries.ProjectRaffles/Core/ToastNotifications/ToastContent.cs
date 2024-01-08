using System;

namespace ProjectIndustries.ProjectRaffles.Core.ToastNotifications
{
  public class ToastContent
  {
    public ToastType Type { get; private set; }
    public string Content { get; private set; }
    public string Title { get; private set; }
    public Action _clickHandler;
    public TimeSpan AutoCloseTimeout { get; private set; }

    public ToastContent(string content, string title, ToastType type, Action clickHandler, TimeSpan autoCloseTimeout)
    {
      Content = content;
      Title = title;
      Type = type;
      _clickHandler = clickHandler;
      AutoCloseTimeout = autoCloseTimeout;
    }

    public static ToastContent Information(string content, string title = "Attention") =>
      new ToastContent(content, title, ToastType.Information, null, TimeSpan.FromSeconds(1.5));

    public static ToastContent Success(string content, string title = "Success") =>
      new ToastContent(content, title, ToastType.Success, null, TimeSpan.FromSeconds(3));

    public static ToastContent Warning(string content, string title = "Caution") =>
      new ToastContent(content, title, ToastType.Warning, null, TimeSpan.FromSeconds(3));

    public static ToastContent Error(string content, string title = "Error") =>
      new ToastContent(content, title, ToastType.Error, null, TimeSpan.FromSeconds(5));
  }
}