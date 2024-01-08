using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.CustomLists;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.CustomLists
{
  public class CustomListsViewModel : PageViewModelBase, IRoutableViewModel
  {
    private readonly ICustomListRepository _repository;
    private readonly IToastNotificationManager _toasts;
    private readonly CustomListHeaderViewModel _header;
    private readonly ReadOnlyObservableCollection<CustomList> _lists;

    public CustomListsViewModel(IMessageBus messageBus, ICustomListRepository repository, IScreen hostScreen,
      IToastNotificationManager toasts, CustomListHeaderViewModel header)
      : base("Custom Lists", messageBus)
    {
      _repository = repository;
      _toasts = toasts;
      _header = header;
      HostScreen = hostScreen;

      var items = repository.Items.Connect();
      items
        .Bind(out _lists)
        .Subscribe();

      items.ToCollection()
        .Where(i => i.Any())
        .Subscribe(i =>
        {
          if (SelectedList == null)
          {
            SelectedList = i.FirstOrDefault();
          }
        });


      var canCreate = this.WhenAnyValue(_ => _.NewListName)
        .Select(grp => !string.IsNullOrWhiteSpace(grp));
      CreateCommand = ReactiveCommand.CreateFromTask(ParseItemsAsync, canCreate);

      var canRemove = this.WhenAnyValue(_ => _.SelectedList).Select(list => list != null);
      RemoveListCommand = ReactiveCommand.CreateFromTask(ct => repository.RemoveAsync(SelectedList), canRemove);
      RemoveItemCommand = ReactiveCommand.CreateFromTask<string>(async (item, ct) =>
      {
        SelectedList.Items.Remove(item);
        await repository.SaveSilentlyAsync(SelectedList, ct);
      });
    }

    protected override ViewModelBase GetHeaderContent() => _header;

    private async Task ParseItemsAsync(CancellationToken ct)
    {
      var items = RawList.Split(new[] {Environment.NewLine, "\n"}, StringSplitOptions.RemoveEmptyEntries)
        .Distinct()
        .ToArray();

      if (!items.Any() || string.IsNullOrWhiteSpace(NewListName))
      {
        _toasts.Show(ToastContent.Error("List can't be empty"));
        return;
      }

      var list = new CustomList(NewListName, items);
      await _repository.SaveAsync(list, ct);
      RawList = null;
      NewListName = null;
    }

    public ReadOnlyObservableCollection<CustomList> Lists => _lists;
    [Reactive] public CustomList SelectedList { get; set; }
    [Reactive] public string RawList { get; set; }
    [Reactive] public string NewListName { get; set; }

    public ReactiveCommand<Unit, Unit> CreateCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> RemoveListCommand { get; private set; }
    public ReactiveCommand<string, Unit> RemoveItemCommand { get; private set; }

    public string UrlPathSegment => nameof(CustomListsViewModel);
    public IScreen HostScreen { get; }
  }
}