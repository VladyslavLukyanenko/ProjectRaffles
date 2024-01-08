using System.Reactive;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Accounts
{
  public class AccountRowViewModel : ViewModelBase
  {
    public AccountRowViewModel(Account account)
    {
      Account = account;
      TogglePasswordVisibility = ReactiveCommand.Create(() =>
      {
        IsPasswordVisible = !IsPasswordVisible;
      });
    }


    public ReactiveCommand<Unit, Unit> TogglePasswordVisibility { get; }

    [Reactive] public bool IsPasswordVisible { get; private set; }
    public Account Account { get; }
  }
}