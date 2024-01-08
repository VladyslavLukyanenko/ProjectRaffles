using System.Runtime.Serialization;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;

using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.Services.PaymentProcessors
{
  [DataContract]
  public class CreditCard : ViewModelBase
  {
    [DataMember, Reactive] public string Cvv { get; set; }
    [DataMember, Reactive] public int ExpirationMonth { get; set; }
    [DataMember, Reactive] public int ExpirationYear { get; set; }
    [DataMember, Reactive] public string Number { get; set; }
  }
}