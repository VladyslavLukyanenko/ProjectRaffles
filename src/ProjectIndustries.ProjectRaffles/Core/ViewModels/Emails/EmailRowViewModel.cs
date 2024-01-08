using System.Reactive;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Emails;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Emails
{
  public class EmailRowViewModel : ViewModelBase
  {
    public EmailRowViewModel(Email email, IEmailRepository emailRepository)
    {
      Email = email;
      RemoveCommand = ReactiveCommand.CreateFromTask(async ct => await emailRepository.RemoveAsync(email, ct));
      TogglePasswordVisibilityCommand = ReactiveCommand.Create(() => { IsPasswordVisible = !IsPasswordVisible; });
    }

    public Email Email { get; }


    [Reactive] public bool IsPasswordVisible { get; private set; }

    public ReactiveCommand<Unit, Unit> RemoveCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> TogglePasswordVisibilityCommand { get; private set; }
  }
}