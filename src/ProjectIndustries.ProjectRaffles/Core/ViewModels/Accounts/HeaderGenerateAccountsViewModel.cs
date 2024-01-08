using System.Reactive;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Accounts;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Accounts
{
  public class HeaderGenerateAccountsViewModel : ImportExportViewModelBase<AccountGroup>
  {
    public HeaderGenerateAccountsViewModel(IDialogService dialogService, IToastNotificationManager toasts,
      IAccountGroupsRepository repository, IAccountImportExportService importExportService) 
      : base(dialogService, toasts, repository, importExportService)
    {
      GenerateCommand = ReactiveCommand.Create(() => { });
    }

    public ReactiveCommand<Unit, Unit> GenerateCommand { get; private set; }
  }
}