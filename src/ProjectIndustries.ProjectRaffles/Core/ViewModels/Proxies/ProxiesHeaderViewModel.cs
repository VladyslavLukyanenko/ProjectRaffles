using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Proxies;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Proxies
{
  public class ProxiesHeaderViewModel : ImportExportViewModelBase<ProxyGroup>
  {
    public ProxiesHeaderViewModel(IDialogService dialogService, IToastNotificationManager toasts,
      IProxyGroupsRepository repository, IProxyImportExportService importExportService)
      : base(dialogService, toasts, repository, importExportService)
    {
    }
  }
}