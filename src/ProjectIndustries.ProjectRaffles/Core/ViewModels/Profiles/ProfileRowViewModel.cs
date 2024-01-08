using System;
using System.Reactive;
using System.Reactive.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Profiles;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Profiles
{
  public class ProfileRowViewModel : ViewModelBase
  {
    public ProfileRowViewModel(Profile profile, IProfilesRepository profilesRepository, RoutingState router,
      ProfileEditorViewModel profileEditor, IClipboardService clipboardService, IToastNotificationManager toasts)
    {
      Profile = profile;
      
      EditCommand = ReactiveCommand.Create(() =>
      {
        profileEditor.Profile = Profile;
        router.Navigate.Execute(profileEditor).Subscribe();
      });
      
      CopyToClipboardCommand = ReactiveCommand.CreateFromTask(async ct =>
      {
        var json = JsonConvert.SerializeObject(Profile, new JsonSerializerSettings
        {
          Formatting = Formatting.Indented,
          ContractResolver = new CamelCasePropertyNamesContractResolver()
        });

        await clipboardService.SetTextAsync(json, ct);
        
        toasts.Show(ToastContent.Success($"Profile '{Profile.ProfileName}' copied to clipboard"));
      });
      
      RemoveCommand = ReactiveCommand.CreateFromTask(async ct =>
      {
        await profilesRepository.RemoveAsync(Profile, ct);
        toasts.Show(ToastContent.Success($"Profile '{Profile.ProfileName}' removed."));
      });

      var onChanges = this.WhenAnyValue(_ => _.Profile);
      onChanges.Select(_ => _?.CreditCard?.Number[^4..]).ToPropertyEx(this, _ => _.Last4Digits);
      onChanges.Select(_ => _?.CreditCard != null).ToPropertyEx(this, _ => _.CreditCardAdded);

      onChanges.Select(_ => _?.ShippingAddress.FullName).ToPropertyEx(this, _ => _.FullName);
      onChanges.Select(_ => _?.ShippingAddress.PhoneNumber).ToPropertyEx(this, _ => _.PhoneNumber);

      onChanges.Select(_ => _?.ProfileName).ToPropertyEx(this, _ => _.ProfileName);
    }

    public Profile Profile { get; }

    public bool CreditCardAdded { [ObservableAsProperty] get; }
    public string Last4Digits { [ObservableAsProperty] get; }
    public string ProfileName { [ObservableAsProperty] get; }
    public string Email { [ObservableAsProperty] get; }
    public string FullName { [ObservableAsProperty] get; }
    public string PhoneNumber { [ObservableAsProperty] get; }

    public ReactiveCommand<Unit, Unit> EditCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> CopyToClipboardCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> RemoveCommand { get; private set; }
  }
}