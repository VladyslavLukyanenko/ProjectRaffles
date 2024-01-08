using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Emails
{
  public class HeaderEmailsViewModel : ViewModelBase
  {
    [Reactive] public string SearchTerm { get; set; }
  }
}