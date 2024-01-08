using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Profiles;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Profiles
{
  public class ProfilesHeaderSearchViewModel : ImportExportViewModelBase<Profile>
  {
    public ProfilesHeaderSearchViewModel(IDialogService dialogService, IToastNotificationManager toasts,
      IProfilesRepository repository, IProfilesImportExportService importExportService)
      : base(dialogService, toasts, repository, importExportService)
    {
    }

    [Reactive] public string SearchTerm { get; set; }
  }
}