using System.Reactive;
using System.Reactive.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Proxies;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Proxies
{
  public class ProxyRowViewModel : ViewModelBase
  {
    public ProxyRowViewModel(Proxy proxy, ProxyGroup proxyGroup, IProxyGroupsRepository proxyGroupsRepository,
      IToastNotificationManager toasts)
    {
      Proxy = proxy;

      RemoveProxyCommand = ReactiveCommand.CreateFromTask(async ct =>
      {
        proxyGroup.RemoveProxy(proxy);
        await proxyGroupsRepository.SaveAsync(proxyGroup);
        toasts.Show(ToastContent.Success("Proxy removed successfully"));
      });
      
      TogglePasswordCommand = ReactiveCommand.Create(() =>
      {
        IsPasswordVisible = !IsPasswordVisible;
      });

      this.WhenAnyValue(_ => _.IsPasswordVisible)
        .Select(isVisible => isVisible ? proxy.Password : "********")
        .ToPropertyEx(this, _ => _.Password);
    }
    
    public string Password { [ObservableAsProperty] get; }

    public Proxy Proxy { get; }
    [Reactive] public bool IsPasswordVisible { get; private set; }
    public ReactiveCommand<Unit, Unit> RemoveProxyCommand { get; }
    public ReactiveCommand<Unit, Unit> TogglePasswordCommand { get; }
  }
}