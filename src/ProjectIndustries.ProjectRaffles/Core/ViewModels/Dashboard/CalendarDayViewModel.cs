using NodaTime;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Dashboard
{
  public class CalendarDayViewModel : ViewModelBase
  {
    public LocalDate Date { get; private set; }
    [Reactive] public bool HasPredefinedRaffles { get; set; }
    [Reactive] public bool IsDisabled { get; set; }
  }
}