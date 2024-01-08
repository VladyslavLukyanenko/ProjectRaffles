using System;
using System.IO;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Diagnostics;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Diagnostics
{
  public class MemoryDumpCreatorViewModel : ViewModelBase
  {
    private readonly IMemoryDumpCreator _memoryDumpCreator;
    private readonly IDialogService _dialogService;
    private readonly IToastNotificationManager _toasts;

    public MemoryDumpCreatorViewModel(IMemoryDumpCreator memoryDumpCreator, IDialogService dialogService,
      IToastNotificationManager toasts)
    {
      _memoryDumpCreator = memoryDumpCreator;
      _dialogService = dialogService;
      _toasts = toasts;

      CreateMemoryDumpCommand = ReactiveCommand.CreateFromTask(CreateMemoryDumpAsync);
    }

    private async Task CreateMemoryDumpAsync(CancellationToken ct)
    {
      var defaultFileName = $"ProjectRafflesMemoryDump_{DateTime.UtcNow:yyyyMMdd_hhMMss}";
      var outputFileName =
        await _dialogService.PickSaveFileAsync("Where would you like to save memory dump?", ".dmp", defaultFileName);

      if (await _memoryDumpCreator.CreateAsync(outputFileName, ct))
      {
        _toasts.Show(ToastContent.Success($"Dump '{Path.GetFileName(outputFileName)}' created successfully."));
        _dialogService.ShowDirectory(Path.GetDirectoryName(outputFileName));
      }
      else
      {
        _toasts.Show(ToastContent.Error("Something went wrong on creating memory dump."));
      }
    }

    public ReactiveCommand<Unit, Unit> CreateMemoryDumpCommand { get; private set; }
  }
}