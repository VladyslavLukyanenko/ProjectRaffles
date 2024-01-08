using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels
{
  public class ValueResolverConfigurationViewModel : ViewModelBase
  {
    public ValueResolverConfigurationViewModel()
    {
      var isOk = this.WhenAnyValue(_ => _.Fields)
        .Select(_ => _.Where(f => f.IsRequired)
          .Select(f => f.Changed)
          .CombineLatest())
        .Switch()
        .Select(fields => fields
          .Select(f => f.IsValid().ToObservable())
          .CombineLatest())
        .Switch()
        .Select(f => f.All(_ => _));

      OkCommand = ReactiveCommand.Create(() => { }, isOk);
    }

    [Reactive] public string Title { get; set; }
    [Reactive] public IList<Field> Fields { get; set; } = new List<Field>();
    public ReactiveCommand<Unit, Unit> OkCommand { get; private set; }
  }
}