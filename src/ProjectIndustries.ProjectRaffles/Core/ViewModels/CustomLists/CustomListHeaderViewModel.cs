using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.CustomLists;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.CustomLists
{
  public class CustomListHeaderViewModel : ImportExportViewModelBase<CustomList>
  {
    public CustomListHeaderViewModel(IDialogService dialogService,
      ICustomListImportExportService listImportExportService, IToastNotificationManager toasts,
      ICustomListRepository repository)
      : base(dialogService, toasts, repository, listImportExportService)
    {
    }
  }
}