using ProjectIndustries.ProjectRaffles.Core.Services.PaymentProcessors;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace ProjectIndustries.ProjectRaffles.Core.Domain
{
  [DataContract]
  public class Profile : ViewModelBase, IEntity
  {
    public Profile()
    {
      DispatchOnChanges(_ => _.ShippingAddress, nameof(ShippingAddress));
      DispatchOnChanges(_ => _.BillingAddress, nameof(BillingAddress));
      DispatchOnChanges(_ => _.CreditCard, nameof(CreditCard));

      void DispatchOnChanges<T>(Expression<Func<Profile, T>> selector, string changedPropName)
        where T : ViewModelBase
      {
        IDisposable onChanged = null;
        this.WhenAnyValue(selector).Subscribe(s =>
        {
          onChanged?.Dispose();
          onChanged = s?.Changed.Subscribe(_ => this.RaisePropertyChanged(changedPropName));
        });
      }
    }

    [DataMember]
    public Guid Id { get; private set; }

    [Reactive, DataMember] public Address BillingAddress { get; set; } = new Address();
    [Reactive, DataMember] public Address ShippingAddress { get; set; } = new Address();
    [Reactive, DataMember] public CreditCard CreditCard { get; set; }
    [Reactive, DataMember] public bool IsShippingSameAsBilling { get; set; }
    [Reactive, DataMember] public string ProfileName { get; set; }
  }
}