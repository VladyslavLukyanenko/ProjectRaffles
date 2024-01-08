using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.ToastNotifications;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels
{
  public abstract class ImportExportViewModelBase<T> : ViewModelBase
    where T : IEntity
  {
    private readonly IDialogService _dialogService;
    private readonly IToastNotificationManager _toasts;
    private readonly IImportExportService _importExportService;

    protected ImportExportViewModelBase(IDialogService dialogService, IToastNotificationManager toasts,
      IRepository<T> repository, IImportExportService importExportService)
    {
      _dialogService = dialogService;
      _toasts = toasts;
      _importExportService = importExportService;

      var canExport = repository.Items.Connect()
        .ToCollection()
        .Select(_ => _.Any());
      ImportGroupsCommand = ReactiveCommand.CreateFromTask(ImportGroupsAsync);
      ExportGroupsToJsonCommand = ReactiveCommand.CreateFromTask(ExportAsJsonAsync, canExport);
      ExportGroupsToCsvCommand = ReactiveCommand.CreateFromTask(ExportAsCsvAsync, canExport);
    }

    private async Task ExportAsCsvAsync(CancellationToken ct)
    {
      var saveFile = await _dialogService.PickSaveFileAsync("Please select file to save exported data", ".csv");
      if (string.IsNullOrEmpty(saveFile))
      {
        return;
      }

      using (var file = File.Create(saveFile))
      {
        await _importExportService.ExportAsCsvAsync(file, ct);
        _toasts.Show(ToastContent.Success("All data exported as CSV."));
      }
    }

    private async Task ExportAsJsonAsync(CancellationToken ct)
    {
      var saveFile = await _dialogService.PickSaveFileAsync("Please select file to save exported data", ".json");
      if (string.IsNullOrEmpty(saveFile))
      {
        return;
      }

      using (var file = File.Create(saveFile))
      {
        await _importExportService.ExportAsJsonAsync(file, ct);
      }

      _toasts.Show(ToastContent.Success("All data exported as JSON."));
    }

    private async Task ImportGroupsAsync(CancellationToken ct)
    {
      var importFilePath =
        await _dialogService.PickOpenFileAsync("Please select file to import data", ".csv", ".json");
      if (string.IsNullOrEmpty(importFilePath))
      {
        return;
      }

      var ext = Path.GetExtension(importFilePath).ToLowerInvariant();

      bool isSuccessful;
      using (var file = File.OpenRead(importFilePath))
      {
        switch (ext)
        {
          case ".csv":
            isSuccessful = await _importExportService.ImportFromCsvAsync(file, ct);
            break;
          case ".json":
            isSuccessful = await _importExportService.ImportFromJsonAsync(file, ct);
            break;
          default:
            _toasts.Show(ToastContent.Error($"Selected file has invalid format: '{ext}'. Supported only JSON and CSV"));
            return;
        }
      }

      if (!isSuccessful)
      {
        _toasts.Show(ToastContent.Warning("Selected file is empty"));
        return;
      }

      _toasts.Show(ToastContent.Success($"All data were imported."));
    }

    public ReactiveCommand<Unit, Unit> ImportGroupsCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> ExportGroupsToCsvCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> ExportGroupsToJsonCommand { get; private set; }
  }
}