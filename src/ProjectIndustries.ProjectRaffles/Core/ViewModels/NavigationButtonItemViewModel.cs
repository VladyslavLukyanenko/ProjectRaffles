using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels
{
    public class NavigationButtonItemViewModel
        : ViewModelBase
    {
        [Reactive] public bool IsActivated { get; set; }
        public string RegularIconSrc { get; set; }
        public string ActiveIconSrc { get; set; }
        public ReactiveCommand<Unit, Unit> Command { get; set; }
    }
}