using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.PaymentProcessors;
using ProjectIndustries.ProjectRaffles.Core.Services.Profiles;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ProjectIndustries.ProjectRaffles.Core.Validators;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Profiles
{
  public class ProfileEditorViewModel : ViewModelBase, IRoutableViewModel
  {
    public ProfileEditorViewModel(ICountriesService countriesService, IProfilesRepository profilesRepository,
      AddressValidator addressValidator, ProfileValidator profileValidator,
      IToastNotificationManager toasts, IScreen hostScreen)
    {
      var profile = this.WhenAnyValue(_ => _.Profile);
      CompositeDisposable disposable = null;
      profile
        .Where(p => p != null)
        .Subscribe(p =>
        {
          disposable?.Dispose();
          disposable = new CompositeDisposable();

          
          ShippingAddress = new AddressEditorViewModel(countriesService, p.ShippingAddress, addressValidator);
          BillingAddress = new AddressEditorViewModel(countriesService, p.BillingAddress, addressValidator);
          p.Changed
            .Throttle(TimeSpan.FromMilliseconds(200))
            .Select(_ => profileValidator.Validate(Profile).IsValid)
            .ToPropertyEx(this, _ => _.IsValid)
            .DisposeWith(disposable);

          p.Changed.Select(_ => p.CreditCard != null)
            .ToPropertyEx(this, _ => _.IsCreditCardCreated)
            .DisposeWith(disposable);
          p.RaisePropertyChanged();
        });

      CancelCommand = ReactiveCommand.Create(() => { HostScreen.Router.NavigateBack.Execute().Subscribe(); });

      var canExecuteSave = new BehaviorSubject<bool>(false);
      SaveChangesCommand = ReactiveCommand.CreateFromTask(async ct =>
      {
        await profilesRepository.SaveAsync(Profile, ct);
        await CancelCommand.Execute().FirstOrDefaultAsync();
        toasts.Show(ToastContent.Success("Changes made to profile were saved"));
      }, canExecuteSave);

      RemoveCreditCard = ReactiveCommand.Create(() => { Profile.CreditCard = null; });
      AddCreditCard = ReactiveCommand.Create(() => { Profile.CreditCard = new CreditCard(); });

      ToggleCvvVisibilityCommand = ReactiveCommand.Create(() => { IsCvvVisible = !IsCvvVisible; });

      this.WhenAnyValue(_ => _.IsValid)
        .ObserveOn(RxApp.MainThreadScheduler)
        .CombineLatest(SaveChangesCommand.IsExecuting, (isValid, isExecuting) => (isValid, isExecuting))
        .Subscribe(p => canExecuteSave.OnNext(p.isValid && !p.isExecuting));
      HostScreen = hostScreen;
    }

    public bool IsCreditCardCreated { [ObservableAsProperty] get; }
    public bool IsValid { [ObservableAsProperty] get; }
    [Reactive] public Profile Profile { get; set; }
    [Reactive] public AddressEditorViewModel ShippingAddress { get; private set; }
    [Reactive] public AddressEditorViewModel BillingAddress { get; private set; }

    [Reactive] public bool IsCvvVisible { get; private set; }


    public string UrlPathSegment => nameof(ProfileEditorViewModel);
    public IScreen HostScreen { get; }


    public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }
    public ReactiveCommand<Unit, Unit> SaveChangesCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> ToggleCvvVisibilityCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> AddCreditCard { get; private set; }
    public ReactiveCommand<Unit, Unit> RemoveCreditCard { get; private set; }
  }
}