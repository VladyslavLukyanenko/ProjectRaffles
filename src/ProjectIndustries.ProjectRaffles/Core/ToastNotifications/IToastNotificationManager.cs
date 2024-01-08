namespace ProjectIndustries.ProjectRaffles.Core.ToastNotifications
{
  public interface IToastNotificationManager
  {
    void Suspend();
    void Resume();
    void Clear();
    void Show(ToastContent content);
  }
}