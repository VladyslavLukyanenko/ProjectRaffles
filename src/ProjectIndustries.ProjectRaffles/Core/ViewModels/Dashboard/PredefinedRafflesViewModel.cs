using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using NodaTime;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Modules;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels.Dashboard
{
  public class PredefinedRafflesViewModel : ViewModelBase
  {
    public PredefinedRafflesViewModel(IRaffleReleaseService raffleReleaseService, TaskEditViewModel taskEditor)
    {
      raffleReleaseService.Specs
        .Subscribe(specs => Specs = specs);

      RefreshSpecsCommand = ReactiveCommand.CreateFromTask<YearMonth>(async (ym, ct) =>
      {
        await raffleReleaseService.RefreshReleasesAsync(ym, ct);
      });

      this.WhenAnyValue(_ => _.YearMonth)
        .InvokeCommand(RefreshSpecsCommand);

      this.WhenAnyValue(_ => _.Specs)
        .Select(s => !s?.Any() ?? false)
        .ToPropertyEx(this, _ => _.NothingFound);

      taskEditor.CloseCommand.Subscribe(_ => { TaskEditor = null; });

      CreateTaskFromSpecCommand = ReactiveCommand.Create<RaffleTaskSpec>(s =>
      {
        taskEditor.Reset();
        var initialValues = s.AdditionalFields.ToDictionary(_ => _.SystemName, _ => _.Value);
        taskEditor.SelectModuleByName(s.ProviderName, initialValues);
        TaskEditor = taskEditor;
        SelectedSpec = s;
      });
    }

    [Reactive] public TaskEditViewModel TaskEditor { get; private set; }
    [Reactive] public RaffleTaskSpec SelectedSpec { get; set; }
    [Reactive] public YearMonth YearMonth { get; set; }
    [Reactive] public IList<RaffleTaskSpec> Specs { get; private set; }
    public bool NothingFound { [ObservableAsProperty] get; }

    [Reactive] public PredefinedRafflesShowPeriod Period { get; set; }
    public ReactiveCommand<YearMonth, Unit> RefreshSpecsCommand { get; }

    public ReactiveCommand<RaffleTaskSpec, Unit> CreateTaskFromSpecCommand { get; set; }
  }
}