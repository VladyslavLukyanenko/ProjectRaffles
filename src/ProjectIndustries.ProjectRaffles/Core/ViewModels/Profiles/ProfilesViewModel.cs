using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Profiles;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Profiles
{
  public class ProfilesViewModel : PageViewModelBase, IRoutableViewModel
  {
    private readonly ProfilesHeaderSearchViewModel _search;
    private readonly ReadOnlyObservableCollection<ProfileRowViewModel> _profiles;

    public ProfilesViewModel(IMessageBus messageBus, IScreen hostScreen, IProfilesRepository profilesRepository,
      ProfilesHeaderSearchViewModel search, ProfileEditorViewModel profileEditor, IClipboardService clipboardService,
      IToastNotificationManager toasts)
      : base("Profiles", messageBus)
    {
      _search = search;
      HostScreen = hostScreen;

      var filter = search.WhenAnyValue(_ => _.SearchTerm)
        .Throttle(TimeSpan.FromMilliseconds(300))
        .ObserveOn(RxApp.MainThreadScheduler)
        .Select(BuildSearch);

      profilesRepository.Items.Connect()
        .Transform(p =>
          new ProfileRowViewModel(p, profilesRepository, hostScreen.Router, profileEditor, clipboardService, toasts))
        .Filter(filter)
        .Bind(out _profiles)
        .Subscribe();

      CreateProfileCommand = ReactiveCommand.Create(() =>
      {
        profileEditor.Profile = new Profile();
        hostScreen.Router.Navigate.Execute(profileEditor).Subscribe();
      });
    }

    protected override ViewModelBase GetHeaderContent() => _search;

    public ReadOnlyObservableCollection<ProfileRowViewModel> Profiles => _profiles;
    public ReactiveCommand<Unit, Unit> CreateProfileCommand { get; private set; }

    public string UrlPathSegment => nameof(ProfilesViewModel);
    public IScreen HostScreen { get; }

    private Func<ProfileRowViewModel, bool> BuildSearch(string searchTerm)
    {
      if (string.IsNullOrWhiteSpace(searchTerm))
      {
        return _ => true;
      }

      return _ => _.Profile.ProfileName.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase);
    }
  }
}