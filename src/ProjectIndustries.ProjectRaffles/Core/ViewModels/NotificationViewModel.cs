using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels
{
  public class NotificationViewModel : ViewModelBase
  {
    [Reactive] public string Title { get; set; }
    [Reactive] public string Description { get; set; }
    [Reactive] public bool IsSuccessful { get; set; }
  }
}
