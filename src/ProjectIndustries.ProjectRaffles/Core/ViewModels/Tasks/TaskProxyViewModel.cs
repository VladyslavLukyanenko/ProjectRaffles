using ProjectIndustries.ProjectRaffles.Core.Domain;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Tasks
{
  public class TaskProxyViewModel : ViewModelBase
  {
    public TaskProxyViewModel(ProxyGroup proxyGroup)
    {
      ProxyGroup = proxyGroup;
    }

    public ProxyGroup ProxyGroup { get; }
    [Reactive] public bool IsEnabled { get; set; }
  }
}